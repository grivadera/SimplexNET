using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProgramaSimplex
{
    public partial class FrmCargaModelo : Form
    {
        private ModeloPL Modelo;
        public FrmCargaModelo()
        {
            InitializeComponent();
        }

        public void Mostrar(ref ModeloPL pModelo)        {

            if (pModelo != null)
            {
                Modelo = pModelo;
                CargarEnPantalla();
            }
            this.ShowDialog();
            pModelo= Modelo;
        }
        private void txtNroRestricciones_Leave(object sender, EventArgs e)
        {
            GenerarGrillas();
        }

        private void GenerarGrillas()
        {
            dGVc.Columns.Clear();
            dGVAb.Columns.Clear();
            dGVAb.Rows.Clear();

            for (int i = 1; i <= int.Parse(txtNroVariables.Text); i++)
            {
                DataGridViewColumn Columna = new DataGridViewTextBoxColumn();
                Columna.HeaderText = "X" + i.ToString();
                Columna.Width = 100;
                Columna.ValueType = Type.GetType("System.Decimal");
                Columna.Name = "X" + i.ToString();
                dGVc.Columns.Add(Columna);
                DataGridViewColumn Columna2 = (DataGridViewTextBoxColumn) Columna.Clone();
                dGVAb.Columns.Add(Columna2);
            }

            dGVc.Rows.Add();

            //RHS
            DataGridViewColumn ColumnaRHS = new DataGridViewTextBoxColumn();
            ColumnaRHS.HeaderText = "RHS";
            ColumnaRHS.Width = 100;
            ColumnaRHS.ValueType = Type.GetType("System.Decimal");
            ColumnaRHS.Name = "RHS";            
            dGVAb.Columns.Add(ColumnaRHS);
            dGVAb.Columns["RHS"].DefaultCellStyle.BackColor = Color.Aqua;

            //filas A
            for (int i = 1; i <= int.Parse(txtNroRestricciones.Text); i++)
            {
                dGVAb.Rows.Add();
            }
        }

        private void CargarModelo()
        {
            int NroVariables = int.Parse(txtNroVariables.Text);
            int NroRestricciones = int.Parse(txtNroRestricciones.Text);
            double[] c = new double[NroVariables];
            double[] b = new double[NroRestricciones];
            double[,] A = new double[NroRestricciones, NroVariables];

            for (int i = 0; i < NroVariables; i++)
            {
                c[i] = double.Parse(dGVc[i, 0].Value.ToString());
            }

            for (int i = 0; i < NroRestricciones; i++)
            {
                b[i] = double.Parse(dGVAb[NroVariables, i].Value.ToString());
            }

            for (int i = 0; i < NroRestricciones; i++)
            {
                for (int j = 0; j < NroVariables; j++)
                {
                    A[i, j] = double.Parse(dGVAb[j, i].Value.ToString());
                }
            }

            Modelo = new ModeloPL(txtNombre.Text,txtArchivo.Text, c, b, A);
        }

        private void btnLeerModelo_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            Modelo = new ModeloPL();
            Modelo.Leer(openFileDialog1.FileName);
            CargarEnPantalla();
        }

        private void CargarEnPantalla()
        {

            txtNombre.Text = Modelo.Nombre;
            txtArchivo.Text = Modelo.Archivo;
            txtNroRestricciones.Text = Modelo.NroRestricciones.ToString();
            txtNroVariables.Text = Modelo.NroVariables.ToString();

            GenerarGrillas();

            //Cargar en grilla
            for (int i = 0; i < Modelo.NroVariables; i++)
            {
                dGVc[i, 0].Value = Modelo.c[i].ToString();
            }

            for (int i = 0; i < Modelo.NroRestricciones; i++)
            {
                dGVAb[Modelo.NroVariables, i].Value = Modelo.b[i].ToString();
            }

            for (int i = 0; i < Modelo.NroRestricciones; i++)
            {
                for (int j = 0; j < Modelo.NroVariables; j++)
                {
                    dGVAb[j, i].Value = Modelo.A[i, j].ToString();
                }
            }


        }

        private void btnGrabarModelo_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            string Archivo = saveFileDialog1.FileName;
            CargarModelo();
            Modelo.Nombre = txtNombre.Text;
            
            Modelo.Grabar(Archivo);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            CargarModelo();
            this.Close();
        }

        private void btnMas_Click(object sender, EventArgs e)
        {
            dGVAb.Rows.Add();
            txtNroRestricciones.Text = (int.Parse(txtNroRestricciones.Text)+1).ToString();
        }

        private void btnMenos_Click(object sender, EventArgs e)
        {
            dGVAb.Rows.RemoveAt(dGVAb.CurrentCell.RowIndex);
            txtNroRestricciones.Text = (int.Parse(txtNroRestricciones.Text)-1).ToString();
        }
    }
}
