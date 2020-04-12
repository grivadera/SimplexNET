using System;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace ProgramaSimplex
{

    public class ModeloPL
    {

        string _Nombre = "";
        string _Archivo = "";
        double[] _c;
        double[] _b;
        double[,] _A;
        int _NroRestricciones = 0;
        int _NroVariables = 0;
        string _Algebraic = "";

        public double[] c
        {
            get
            {
                return _c;
            }

            set
            {
                _c = value;
            }
        }

        public double[] b
        {
            get
            {
                return _b;
            }

            set
            {
                _b = value;
            }
        }

        public double[,] A
        {
            get
            {
                return _A;
            }

            set
            {
                _A = value;
            }
        }

        public int NroRestricciones
        {
            get
            {
                return _NroRestricciones;
            }

            set
            {
                _NroRestricciones = value;
            }
        }

        public int NroVariables
        {
            get
            {
                return _NroVariables;
            }

            set
            {
                _NroVariables = value;
            }
        }

        public string Nombre
        {
            get
            {
                return _Nombre;
            }

            set
            {
                _Nombre = value;
            }
        }

        public string Archivo
        {
            get
            {
                return _Archivo;
            }

            set
            {
                _Archivo = value;
            }
        }

        // Constructor con argumentos
        public ModeloPL(string Nombre, string Archivo, double[] c, double[] b, double[,] A)
        {
            _Nombre = Nombre;
            _Archivo = Archivo;
            _c = c;
            _b = b;
            _A = A;
            _NroRestricciones = _b.Length;
            _NroVariables = _c.Length;
        }

        // Constructor sin argumentos
        public ModeloPL()
        {
        }

        public void Grabar(string fileName)
        {
            string Linea = "";
            StreamWriter writer = new StreamWriter(fileName);
            writer.Write("Modelo=" + Nombre + "\r\n");
            writer.Write("NroRestricciones=" + _NroRestricciones.ToString() + "\r\n");
            writer.Write("NroVariables=" + _NroVariables.ToString() + "\r\n");
            writer.Write("c=\r\n");
            for (int i = 0; i < _NroVariables; i++)
            {
                Linea = Linea + _c[i].ToString() + ",";
            }
            Linea = Linea.Substring(0, Linea.Length - 1);
            writer.Write(Linea);
            Linea = "";
            writer.Write("\r\n");
            writer.Write("b=\r\n");
            for (int i = 0; i < _NroRestricciones; i++)
            {
                Linea = Linea + _b[i].ToString() + ",";
            }
            Linea = Linea.Substring(0, Linea.Length - 1);
            writer.Write(Linea);
            writer.Write("\r\n");
            writer.Write("A=\r\n");
            for (int i = 0; i < _NroRestricciones; i++)
            {
                Linea = "";
                for (int j = 0; j < _NroVariables; j++)
                {
                    Linea = Linea + _A[i, j].ToString() + ",";
                }
                Linea = Linea.Substring(0, Linea.Length - 1);
                writer.Write(Linea);
                writer.Write("\r\n");
            }
            writer.Write("\r\n");
            writer.Write("fin\r\n");
            writer.Close();
        }

        internal void Leer(string fileName)
        {

            string Linea;

            try
            {
                StreamReader reader = new StreamReader(fileName);
                Linea = reader.ReadLine();
                _Nombre = Linea.Substring(Linea.IndexOf('=') + 1, Linea.Length - Linea.IndexOf('=') - 1);
                _Archivo = fileName;
                Linea = reader.ReadLine();
                _NroRestricciones = int.Parse(Linea.Substring(Linea.IndexOf('=') + 1, Linea.Length - Linea.IndexOf('=') - 1));
                Linea = reader.ReadLine();
                _NroVariables = int.Parse(Linea.Substring(Linea.IndexOf('=') + 1, Linea.Length - Linea.IndexOf('=') - 1));
                //Validar c
                Linea = reader.ReadLine();

                Linea = reader.ReadLine();
                string[] c = Linea.Split(',');
                _c = new double[_NroVariables];
                for (int i = 0; i < _NroVariables; i++)
                {
                    _c[i] = double.Parse(c[i]);
                }
                //Validar b
                Linea = reader.ReadLine();
                _b = new double[_NroRestricciones];
                Linea = reader.ReadLine();
                string[] b = Linea.Split(',');
                for (int i = 0; i < _NroRestricciones; i++)
                {
                    _b[i] = double.Parse(b[i]);
                }
                //Validar A
                Linea = reader.ReadLine();
                _A = new double[_NroRestricciones, _NroVariables];
                for (int i = 0; i < _NroRestricciones; i++)
                {
                    Linea = reader.ReadLine();
                    string[] Av = Linea.Split(',');
                    for (int j = 0; j < _NroVariables; j++)
                    {
                        _A[i, j] = double.Parse(Av[j]);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar archivo", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public string Algebraica()
        {
            _Algebraic = "Modelo " + Nombre + "\r\n";
            _Algebraic = _Algebraic + "\r\n";
            _Algebraic = _Algebraic + "Func Obj MAX = ";
            

            for (int i = 0; i < _NroVariables; i++)
            {
                _Algebraic = _Algebraic + _c[i].ToString("F3") + " * X" + i.ToString() + " + ";
            }

            _Algebraic = _Algebraic.Substring(0, _Algebraic.Length - 3);

            _Algebraic = _Algebraic + "\r\n";
            _Algebraic = _Algebraic + "Sujeto a : \r\n";
            _Algebraic = _Algebraic + "\r\n";

            for (int i = 0; i < _NroRestricciones; i++)
            {
                for (int j = 0; j < _NroVariables; j++)
                {
                    _Algebraic = _Algebraic + "\t" + _A[i, j].ToString("F3") + " * X" + j + " + ";
                }
                _Algebraic = _Algebraic.Substring(0, _Algebraic.Length - 3);

                _Algebraic = _Algebraic + "\t <= \t" + _b[i].ToString("F3") + "\r\n";

            }
            _Algebraic = _Algebraic + "\r\n";
            return (_Algebraic);

        }

    }
}
