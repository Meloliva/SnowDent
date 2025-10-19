using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Capa_de_Datos;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UPacientesConTratamientos.xaml
    /// </summary>
    public partial class UPacientesConTratamientos : UserControl
    {
        private DReporteRecepcionista dReporte = new DReporteRecepcionista();

        public UPacientesConTratamientos()
        {
            InitializeComponent();
            CargarPacientesConTratamientos();
        }

        private void CargarPacientesConTratamientos()
        {
            var pacientes = dReporte.PacientesConTratamientosEnCurso();
            dgPacientesConTratamientos.ItemsSource = null;///por verse
            dgPacientesConTratamientos.ItemsSource = pacientes;
        }
    }
}
