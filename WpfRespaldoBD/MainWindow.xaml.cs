using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace WpfRespaldoBD
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string nombreArchivoRespaldo { get; set; }
        public string nombreArchivoRestauracion { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }
       
        private void Respaldabutton_Click(object sender, RoutedEventArgs e)
        {
            string _nombreServidor = RespaldarNombreServidorTextBox.Text;
            string _nombreBD = RespaldarNombreBDTextBox.Text;
           
            string _cnx = "Server =" + _nombreServidor + ";Initial Catalog=" + _nombreBD + ";Integrated Security=SSPI;Trusted_Connection=yes;";         
            SqlConnection conexion = new SqlConnection(_cnx );

            string comandoCopia = "BACKUP DATABASE ["+ _nombreBD +"] TO  DISK = N'"+ nombreArchivoRespaldo + "' WITH NOFORMAT, NOINIT,  NAME = N'"+ _nombreBD +"-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";           
            SqlCommand cmd = new SqlCommand(comandoCopia, conexion);
            try
            {
                conexion.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Copia correcta");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "Copia incorrecta");
            }
            finally
            {
                conexion.Close();
                conexion.Dispose();
            }
         
        }

        private void Restaurarbutton_Click(object sender, RoutedEventArgs e)
        {

            string _nombreServidor = RestaurarNombreServidorTextBox.Text;
            string _nombreBD = RestaurarNombreBaseDatosTextBox.Text;


            string sBackup = "RESTORE DATABASE " + this.RestaurarNombreBaseDatosTextBox.Text +
                            " FROM DISK = '" + nombreArchivoRestauracion  + "'" +
                             " WITH REPLACE";

            //string sBackup = "RESTORE DATABASE " + "SIAPlocal" +
            //                " FROM DISK = '" + "F:\\28-4-2016-15-9-SIAP.bak" + "'" +
            //                " WITH REPLACE";
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            //csb.DataSource = "(LocalDb)\\v11.0";
            csb.DataSource = RestaurarNombreServidorTextBox.Text;
            // Es mejor abrir la conexión con la base Master
            csb.InitialCatalog = "master";
            csb.IntegratedSecurity = true;
            //csb.ConnectTimeout = 480; // el predeterminado es 15

            using (SqlConnection con = new SqlConnection(csb.ConnectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmdBackUp = new SqlCommand(sBackup, con);
                    cmdBackUp.ExecuteNonQuery();
                    MessageBox.Show("Se ha restaurado la copia de la base de datos.",
                                    "Restaurar base de datos"
                                    );
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                                    "Error al restaurar la base de datos"
                                    );
                }
            }
           
        }

        private void ExaminarRespaldarButton_Click(object sender, RoutedEventArgs e)
        {
            String nombreCopia = (System.DateTime.Today.Day.ToString() + "-" + System.DateTime.Today.Month.ToString() + "-" + System.DateTime.Today.Year.ToString() + "-" + System.DateTime.Now.Hour.ToString() + "-" + System.DateTime.Now.Minute.ToString() + "-SIAP.bak");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivo de restauracion (*.bak)|*.bak";
            saveFileDialog.FileName = nombreCopia;
            if (saveFileDialog.ShowDialog() == true)
            {
                nombreArchivoRespaldo =  saveFileDialog.FileName;
                RespaldarNombreArchivoTextBox.Text = nombreArchivoRespaldo;
            }
        }

        private void ExaminarRestaurarButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivo de restauracion (*.bak)|*.bak";
            if (openFileDialog.ShowDialog() == true)
            {
               nombreArchivoRestauracion  = openFileDialog.FileName;
                RestaurarNombreArchivoTextBox.Text = nombreArchivoRestauracion ;
            }
        }
    }
}
