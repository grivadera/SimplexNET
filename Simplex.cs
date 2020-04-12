using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramaSimplex
{

    /*************************************************************************
     *  Clase Simplex.cs
     *  Adaptada por Gustavo Rivadera a partir del programa original en Java de:
     *  Robert Sedgewick
     *  Kevin Wayne
     *  Algorithms 4ed.
     *  https://algs4.cs.princeton.edu/code/edu/princeton/cs/algs4/LinearProgramming.java
     *  
     *  Dada un matriz MxN A, un vector b de longitud M, y un 
     *  vector c de longitud M, se resuelve la PL { max cx : Ax <= b, x >= 0 }.
     *  Se asume que b >= 0 de tal forma que xi = 0 es una solución basica factible.
     *
     *  Se crea una tabla simplex de (M+1)-por-(N+M+1) con el lado derecho (RHS) 
     *  en la columna M+N, la funcion objetivo está en la fila M, y las 
     *  variables en las columnas M hasta la M+N-1.
     *
     *************************************************************************/

    public class Simplex
    {
        private const double EPSILON = 1.0E-10;
        private double[,] a;    // tabla a
        private int M;          // numero de restricciones
        private int N;          // numero original de variables

        private int[] basis;    // basis[i] = variable basica correspondiente a la fila i

        private string _Resultado = "";
        private string _PasoAPaso = "";
        public string Resultado
        {
            get { return _Resultado; }
            set { _Resultado = value; }
        }

        public string PasoAPaso
        {
            get { return _PasoAPaso; }
            set { _PasoAPaso = value; }
        }

        private int NroIteraciones = 0;
        // Establecer la matriz (tableaux) inicial del Simplex a partir de vectores
        public Simplex(double[,] A, double[] b, double[] c)
        {
            M = b.Length;
            N = c.Length;
            a = new double[M + 1, N + M + 1];
            for (int i = 0; i < M; i++)
                for (int j = 0; j < N; j++)
                    a[i, j] = A[i, j];
            for (int i = 0; i < M; i++) a[i, N + i] = 1.0;
            for (int j = 0; j < N; j++) a[M, j] = c[j];
            for (int i = 0; i < M; i++) a[i, M + N] = b[i];

            basis = new int[M];
            for (int i = 0; i < M; i++) basis[i] = N + i;

            solve();

            // comprobar condiciones de optimalidad
            //assert check(A, b, c);
        }

        // Establecer la matriz (tableaux) inicial del Simplex a partir de objeto modelo
        public Simplex(ModeloPL Modelo)
        {
            M = Modelo.NroRestricciones;
            N = Modelo.NroVariables;
            a = new double[M + 1, N + M + 1];
            for (int i = 0; i < M; i++)
                for (int j = 0; j < N; j++)
                    a[i, j] = Modelo.A[i, j];
            for (int i = 0; i < M; i++) a[i, N + i] = 1.0;
            for (int j = 0; j < N; j++) a[M, j] = Modelo.c[j];
            for (int i = 0; i < M; i++) a[i, M + N] = Modelo.b[i];

            basis = new int[M];
            for (int i = 0; i < M; i++) basis[i] = N + i;

            _PasoAPaso = "";
            _PasoAPaso = _PasoAPaso + "-------------------------------------------------------------------------\r\n";
            _PasoAPaso = _PasoAPaso + "Matriz Simplex inicial: \r\n";

            _PasoAPaso = _PasoAPaso + "Colum." + "\t";
            for (int j = 0; j <= M + N; j++)
            {
                _PasoAPaso = _PasoAPaso + (j == M + N ? "Sol." : (j >= N && j < M + N ? "s" : "x") + j.ToString()) + "\t";
            }
            _PasoAPaso = _PasoAPaso + "\r\n";

            for (int i = 0; i <= M; i++)
            {
                _PasoAPaso = _PasoAPaso + (i == M ? "Z" : "f" + i.ToString() + " " + (basis[i] < N ? "x" : "s") + basis[i].ToString()) + "\t";

                for (int j = 0; j <= M + N; j++)
                {
                    _PasoAPaso = _PasoAPaso + a[i, j].ToString("F3") + "\t";
                }
                _PasoAPaso = _PasoAPaso + "\r\n";
            }

            solve();

        }

        // correr el algoritmo simplex comenzando con la SFI inicial 
        private void solve()
        {

            while (true) // ciclo iterativo (hasta que no haya más columnas posibles entrantes..ver Condición de optimalidad)
            {

                // Condición de optimalidad
                // Encontrar la columna entrante q
                int q = OptimCond();
                if (q == -1) break;  // Si se dá esta condición (no hay más columnas posibles de entrada) se encontró el óptimo

                NroIteraciones++;    // Incrementar el número de iteraciones

                _PasoAPaso = _PasoAPaso + "-------------------------------------------------------------------------\r\n";
                _PasoAPaso = _PasoAPaso + "Iteración Nº " + NroIteraciones.ToString() + "\r\n";

                //Condición de factibilidad
                // encontrar la fila saliente p
                int p = FactibCond(q);

                if (p == -1) throw new ArithmeticException("Solución No Acotada!!");

                _PasoAPaso = _PasoAPaso + "pivot : Columna=" + q.ToString() + " - Fila=" + p.ToString() + "\r\n";
                // pivot
                pivot(p, q);

                // Actualizar la base
                basis[p] = q;

                _PasoAPaso = _PasoAPaso + " Optimo actual = " + valueopt().ToString() + "\r\n";

                _PasoAPaso = _PasoAPaso + " Base Actual = \t";
                for (int i = 0; i < M; i++)
                    _PasoAPaso = _PasoAPaso + (basis[i] < N ? "x" : "s") + basis[i] + " = " + a[i, M + N].ToString() + (i < M - 1 ? " - " : ""); //if (basis[i] < N)


                _PasoAPaso = _PasoAPaso + "\r\n";

                _PasoAPaso = _PasoAPaso + "Matriz Simplex: \r\n";

                _PasoAPaso = _PasoAPaso + "Colum." + "\t";
                for (int j = 0; j <= M + N; j++)
                {
                    _PasoAPaso = _PasoAPaso + (j == M + N ? "Sol." : (j >= N && j < M + N ? "s" : "x") + j.ToString()) + "\t";
                }
                _PasoAPaso = _PasoAPaso + "\r\n";

                for (int i = 0; i <= M; i++)
                {
                    _PasoAPaso = _PasoAPaso + (i == M ? "Z" : "f" + i.ToString() + " " + (basis[i] < N ? "x" : "s") + basis[i].ToString()) + "\t";

                    for (int j = 0; j <= M + N; j++)
                    {
                        _PasoAPaso = _PasoAPaso + a[i, j].ToString("F3") + "\t";
                    }
                    _PasoAPaso = _PasoAPaso + "\r\n";
                }

            }
        }

        // indice menor de una columna no básica con un costo positivo
        private int OptimCond()
        {
            for (int j = 0; j < M + N; j++)
                if (a[M, j] > 0) return j;
            return -1;  // optimo
        }

        // índice de una columna no básica con el costo más positivo
        private int dantzig()
        {
            int q = 0;
            for (int j = 1; j < M + N; j++)
                if (a[M, j] > a[M, q]) q = j;

            if (a[M, q] <= 0) return -1;  // optimo
            else return q;
        }

        // encontrar una fila p usando la regla de la relación (ratio) mínimo (se devuelve -1 si no existe tal fila)
        private int FactibCond(int q)
        {
            int p = -1;
            for (int i = 0; i < M; i++)
            {
                if (a[i, q] <= 0) continue;
                else if (p == -1) p = i;
                else if ((a[i, M + N] / a[i, q]) < (a[p, M + N] / a[p, q])) p = i;
            }
            return p;
        }

        // Realizar el pivoteo sobre la celda (p,q) usando la eliminacion de Gauss-Jordan
        private void pivot(int p, int q)
        {

            // todo salvo la fila p y la columna q 
            for (int i = 0; i <= M; i++)
                for (int j = 0; j <= M + N; j++)
                    if (i != p && j != q) a[i, j] -= a[p, j] * a[i, q] / a[p, q];

            // poner en cero la columna q
            for (int i = 0; i <= M; i++)
                if (i != p) a[i, q] = 0.0;

            // escalar la fila p
            for (int j = 0; j <= M + N; j++)
                if (j != q) a[p, j] /= a[p, q];
            a[p, q] = 1.0;
        }

        // Devuelve el valor objetivo óptimo
        public double valueopt()
        {
            return -a[M, M + N];
        }

        // Devuelve el vector solución primal 
        public double[] primal()
        {
            double[] x = new double[N];
            for (int i = 0; i < M; i++)
                if (basis[i] < N) x[basis[i]] = a[i, M + N];
            return x;
        }

        // Devuelve el vector solución dual 
        public double[] dual()
        {
            double[] y = new double[M];
            for (int i = 0; i < M; i++)
                y[i] = -a[M, N + i];
            return y;
        }


        // Función usada para ver si la solución primal es factible
        private bool isPrimalFeasible(double[,] A, double[] b)
        {
            double[] x = primal();

            // comprobar que x >= 0
            for (int j = 0; j < x.Length; j++)
            {
                if (x[j] < 0.0)
                {
                    //StdOut.println("x[" + j + "] = " + x[j] + " es negativo");
                    return false;
                }
            }

            // comprobar que Ax <= b
            for (int i = 0; i < M; i++)
            {
                double sum = 0.0;
                for (int j = 0; j < N; j++)
                {
                    sum += A[i, j] * x[j];
                }
                if (sum > b[i] + EPSILON)
                {
                    //StdOut.println("no hay primal factible");
                    //StdOut.println("b[" + i + "] = " + b[i] + ", sum = " + sum);
                    return false;
                }
            }
            return true;
        }

        // Función usada para ver si la solución dual es factible
        private bool isDualFeasible(double[,] A, double[] c)
        {
            double[] y = dual();

            // comprobar que y >= 0
            for (int i = 0; i < y.Length; i++)
            {
                if (y[i] < 0.0)
                {
                    //StdOut.println("y[" + i + "] = " + y[i] + " is negative");
                    return false;
                }
            }

            // comprobar que yA >= c
            for (int j = 0; j < N; j++)
            {
                double sum = 0.0;
                for (int i = 0; i < M; i++)
                {
                    sum += A[i, j] * y[i];
                }
                if (sum < c[j] - EPSILON)
                {
                    //StdOut.println("no hay dual factible");
                    //StdOut.println("c[" + j + "] = " + c[j] + ", sum = " + sum);
                    return false;
                }
            }
            return true;
        }

        // comprobar que el valor optimo = cx = yb
        private bool isOptimal(double[] b, double[] c)
        {
            double[] x = primal();
            double[] y = dual();
            double value = valueopt();

            // comprobar que = cx = yb
            double value1 = 0.0;
            for (int j = 0; j < x.Length; j++)
                value1 += c[j] * x[j];
            double value2 = 0.0;
            for (int i = 0; i < y.Length; i++)
                value2 += y[i] * b[i];
            if (Math.Abs(value - value1) > EPSILON || Math.Abs(value - value2) > EPSILON)
            {
                //StdOut.println("valor = " + value + ", cx = " + valor1 + ", yb = " + value2);
                return false;
            }

            return true;
        }

        // Comprobar opcionalmente factibilidad y optimalidad
        private bool check(double[,] A, double[] b, double[] c)
        {
            return isPrimalFeasible(A, b) && isDualFeasible(A, c) && isOptimal(b, c);
        }

        // Mostrar resultados completos
        public void show()
        {
            _Resultado = "";
            _Resultado = "Resultado \r\n";
            _Resultado = _Resultado + "M = " + M + "\r\n"; // StdOut.println("M = " + M);
            _Resultado = _Resultado + "N = " + N + "\r\n";
            _Resultado = _Resultado + "-------------------------------------------------------------------------\r\n";
            for (int i = 0; i <= M; i++)
            {
                for (int j = 0; j <= M + N; j++)
                {
                    _Resultado = _Resultado + a[i, j].ToString("F3") + "\t";
                }
                _Resultado = _Resultado + "\r\n";
            }
            _Resultado = _Resultado + "value = " + valueopt().ToString() + "\r\n";
            for (int i = 0; i < M; i++)
                if (basis[i] < N) _Resultado = _Resultado + "x_" + basis[i] + " = " + a[i, M + N].ToString() + "\r\n";
            _Resultado = _Resultado + "\r\n";
        }

    }


}
