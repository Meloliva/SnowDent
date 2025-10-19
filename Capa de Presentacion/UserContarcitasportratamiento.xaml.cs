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
using Capa_de_Datos;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UserContarcitasportratamiento.xaml
    /// </summary>
    public partial class UserContarcitasportratamiento : UserControl
    {
        public UserContarcitasportratamiento()
        {
            InitializeComponent();
            CargarDatos();

        }
        private void CargarDatos()
        {
            DReporteRecepcionista reporte = new DReporteRecepcionista();
            var datos = reporte.TotalCitasPorTipoTratamiento();

            foreach (var item in datos)
            {
                switch (item.NombreTratamiento.ToLower())
                {
                    case "ortodoncia":
                        lblOrtodoncia.Content = item.TotalCitas.ToString();
                        break;
                    case "implantes":
                        lblImplantes.Content = item.TotalCitas.ToString();
                        break;
                    case "cirugias":
                        lblCirugias.Content = item.TotalCitas.ToString();
                        break;
                    case "odontopediatria":
                        lblPediatria.Content = item.TotalCitas.ToString();
                        break;
                    case "estetica dental":
                        lblEstetica.Content = item.TotalCitas.ToString();
                        break;
                    case "odontogeriatria":
                        lblGeriatria.Content = item.TotalCitas.ToString();
                        break;
                    default:
        
                        break;
                }

            }
        }

    }
}
