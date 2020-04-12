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
    public partial class Principal : Form
    {
        ModeloPL Modelo;
        public Principal()
        {
            InitializeComponent();
        }

    public void InformeCompleto(double[,] A, double[] b, double[] c) {

        Simplex lp = new Simplex(A, b, c);
        txtResultado.Text = "Resultado : \r\n";
        txtResultado.Text = txtResultado.Text + "-------------------------------------------------------------------------\r\n";
        txtResultado.Text = txtResultado.Text + "Optimo = " + lp.valueopt().ToString() + "\r\n"; 

        double[] x = lp.primal();
        txtResultado.Text = txtResultado.Text + "Primal : \r\n"; 
        for (int i = 0; i < x.Length; i++)
            txtResultado.Text = txtResultado.Text + "x[" + i + "] = " + x[i].ToString() + "\r\n";

        double[] y = lp.dual();
        txtResultado.Text = txtResultado.Text + "Dual : \r\n"; 
        for (int j = 0; j < y.Length; j++)
            txtResultado.Text = txtResultado.Text + "y[" + j + "] = " + y[j].ToString() + "\r\n";

        lp.show();
        txtResultado.Text = txtResultado.Text + lp.Resultado;
    }

    public void InformeCompleto(ModeloPL Modelo)
        {
            this.toolStripStatusLabel1.Text = "Modelo " + Modelo.Nombre;

            Simplex lp = new Simplex(Modelo);
            txtResultado.Text = "Resultado : \r\n";
            txtResultado.Text = txtResultado.Text + "-------------------------------------------------------------------------\r\n";
            txtResultado.Text = txtResultado.Text + "Optimo = " + lp.valueopt().ToString() + "\r\n";

            double[] x = lp.primal();
            txtResultado.Text = txtResultado.Text + "Primal : \r\n";
            for (int i = 0; i < x.Length; i++)
                txtResultado.Text = txtResultado.Text + "x[" + i + "] = " + x[i].ToString() + "\r\n";

            double[] y = lp.dual();
            txtResultado.Text = txtResultado.Text + "Dual : \r\n";
            for (int j = 0; j < y.Length; j++)
                txtResultado.Text = txtResultado.Text + "y[" + j + "] = " + y[j].ToString() + "\r\n";

            lp.show();
            txtResultado.Text = txtResultado.Text + lp.Resultado;

        }
        public void InformeCompleto1() {
            double[,] A = {
            { -1,  1,  0 },
            {  1,  4,  0 },
            {  2,  1,  0 },
            {  3, -4,  0 },
            {  0,  0,  1 },
            };
            double[] c = { 1, 1, 1 };
            double[] b = { 5, 45, 27, 24, 4 };
            Modelo = new ModeloPL("Ejemplo1", "-", c, b, A);

        InformeCompleto(Modelo);
    }


    // x0 = 12, x1 = 28, opt = 800
    public void InformeCompleto2() {
            double[] c = {  13.0,  23.0 };
            double[] b = { 480.0, 160.0, 1190.0 };
            double[,] A = {
            {  5.0, 15.0 },
            {  4.0,  4.0 },
            { 35.0, 20.0 },
        };
        Modelo = new ModeloPL("Ejemplo2", "-", c, b, A);
        InformeCompleto(Modelo);
    }

    // No acotada
    public void InformeCompleto3() {
        double[] c = { 2.0, 3.0, -1.0, -12.0 };
        double[] b = {  3.0,   2.0 };
        double[,] A = {
            { -2.0, -9.0,  1.0,  9.0 },
            {  1.0,  1.0, -1.0, -2.0 },
        };
        Modelo = new ModeloPL("Ejemplo2", "-", c, b, A);
        InformeCompleto(Modelo);
    }

    // Degeneración - Se hace un ciclo si se elige el coeficiente mas positivo
    public void InformeCompleto4() {

            double[] c = { 10.0, -57.0, -9.0, -24.0 };
            double[] b = {  0.0,   0.0,  1.0 };
            double[,] A = {
            { 0.5, -5.5, -2.5, 9.0 },
            { 0.5, -1.5, -0.5, 1.0 },
            { 1.0,  0.0,  0.0, 0.0 },
            };
            Modelo = new ModeloPL("Degeneración", "-", c, b, A);
            InformeCompleto(Modelo);

    }

        public void ReddyMikks()
        {
            double[] c = { 5.0, 4.0 };
            double[] b = { 24.0, 6.0, 1.0, 2.0 };
            double[,] A = {
            { 6.0, 4.0 },
            { 1.0, 2.0 },
            { -1.0,  1.0 },
            { 0.0,  1.0 }
        };
            Modelo = new ModeloPL("Reddy Mikks", "-", c, b, A);
            InformeCompleto(Modelo);
        }


        // InformeCompleto client
        public void Probar() {

        try                           { ReddyMikks();             }
        catch (ArithmeticException e) { txtResultado.Text = txtResultado.Text +  e.StackTrace; }
        txtResultado.Text = txtResultado.Text + "-------------------------------------------------------------------------";
        /*
        try                           { InformeCompleto2();             }
        catch (ArithmeticException e) { txtResultado.Text = txtResultado.Text +  e.StackTrace; }
        txtResultado.Text = txtResultado.Text + "--------------------------------";

        try                           { InformeCompleto3();             }
        catch (ArithmeticException e) { txtResultado.Text = txtResultado.Text +  e.StackTrace; }
        txtResultado.Text = txtResultado.Text + "--------------------------------";

        try                           { InformeCompleto4();             }
        catch (ArithmeticException e) { txtResultado.Text = txtResultado.Text +  e.StackTrace; }
        txtResultado.Text = txtResultado.Text + "--------------------------------";
        */
    }

        private void button1_Click(object sender, EventArgs e)
        {
            //Probar();

        }

        private void leerArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCargaModelo f = new FrmCargaModelo();
            f.Mostrar(ref Modelo);
            this.toolStripStatusLabel1.Text = "Modelo " + Modelo.Nombre;
            //InformeCompleto(Modelo);
        }

        private void resoluciónPrimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Simplex lp = new Simplex(Modelo);
            txtResultado.Text = "Resultado : \r\n";
            txtResultado.Text = txtResultado.Text + "-------------------------------------------------------------------------\r\n";
            txtResultado.Text = txtResultado.Text + "Optimo = " + lp.valueopt().ToString() + "\r\n";

            double[] x = lp.primal();
            txtResultado.Text = txtResultado.Text + "Solución Primal : \r\n";
            for (int i = 0; i < x.Length; i++)
                txtResultado.Text = txtResultado.Text + "x[" + i + "] = " + x[i].ToString() + "\r\n";
            
            //ReddyMikks();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iteracionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Simplex lp = new Simplex(Modelo);
            double[] x = lp.primal();
            txtResultado.Text = lp.PasoAPaso;
        }

        private void formAlgebraicaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Modelo != null) txtResultado.Text = Modelo.Algebraica();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            leerArchivoToolStripMenuItem_Click(this, e);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            resoluciónPrimalToolStripMenuItem_Click(this, e);
        }

        private void resoluciónDualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Simplex lp = new Simplex(Modelo);
            txtResultado.Text = "Resultado : \r\n";
            txtResultado.Text = txtResultado.Text + "-------------------------------------------------------------------------\r\n";
            txtResultado.Text = txtResultado.Text + "Optimo = " + lp.valueopt().ToString() + "\r\n";

            double[] y = lp.dual();
            txtResultado.Text = txtResultado.Text + "Dual : \r\n";
            for (int j = 0; j < y.Length; j++)
                txtResultado.Text = txtResultado.Text + "y[" + j + "] = " + y[j].ToString() + "\r\n";

        }

        private void informeCompletoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Simplex lp = new Simplex(Modelo);
            txtResultado.Text = "Resultado : \r\n";
            txtResultado.Text = txtResultado.Text + "-------------------------------------------------------------------------\r\n";
            txtResultado.Text = txtResultado.Text + "Optimo = " + lp.valueopt().ToString() + "\r\n";

            double[] x = lp.primal();
            txtResultado.Text = txtResultado.Text + "Primal : \r\n";
            for (int i = 0; i < x.Length; i++)
                txtResultado.Text = txtResultado.Text + "x[" + i + "] = " + x[i].ToString() + "\r\n";

            double[] y = lp.dual();
            txtResultado.Text = txtResultado.Text + "Dual : \r\n";
            for (int j = 0; j < y.Length; j++)
                txtResultado.Text = txtResultado.Text + "y[" + j + "] = " + y[j].ToString() + "\r\n";

            lp.show();
            txtResultado.Text = txtResultado.Text + lp.Resultado;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            resoluciónDualToolStripMenuItem_Click(this, e);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            iteracionesToolStripMenuItem_Click(this, e);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            salirToolStripMenuItem_Click(this, e);
        }

        private void ejemplo1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InformeCompleto1();
        }

        private void ejemplo2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InformeCompleto2();
        }

        private void noAcotadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InformeCompleto3();
        }

        private void ciclosInfinitosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InformeCompleto4();
        }

        private void reddyMikksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReddyMikks();
        }
    }
}
