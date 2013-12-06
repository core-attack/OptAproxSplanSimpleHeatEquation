using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using zg = ZedGraph;


namespace OptAproxSplanSimpleHeatEquation
{
    public partial class GeneralForm : Form
    {
        System.Globalization.NumberFormatInfo format;
        Parser p = new Parser();
        string fileEq = Application.StartupPath + "allEquations.txt";
        Encoding myEnc = Encoding.UTF8;
        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-us");
        string formatInfo = "000,000,000,000.00###";
        string sFormat = "{0:0.0000000000000000000}";

        public GeneralForm()
        {
            InitializeComponent();
            setFormatSep();
            setRtb();
            setCheckBox();
            setComboBox();
            forEq();
            labelfirst.Text = labelsecond.Text = labelthird.Text = labelfourth.Text = labelResult.Text = "не вычислено";
            richTextBoxLog.Cursor = Cursors.IBeam;
            N = N_;
            n = n_;
            h = h__;
            GAMMA = GAMMA_;
            R = R_;
            TAU = TAU_;
            TETA = TETA_;
            x = x_;
            alpha = alpha_;
            beta = beta_;
        }

        //для уравнений
        void forEq()
        {
            if (!File.Exists(fileEq))
                File.WriteAllText(fileEq, "", myEnc);
            readFileEq();

            foreach (string s in listFi)
                //if (!isHave(listFi, s))
                    comboBoxFi.Items.Add(s);
            foreach (string s in listRo0)
                //if (!isHave(listRo0, s))
                    comboBoxRo0.Items.Add(s);
            foreach (string s in listRo1)
                //if (!isHave(listRo1, s))
                    comboBoxRo1.Items.Add(s);

        }

        //для ричтекстбоксов
        void setRtb()
        {
            oldW = Width;
        }

        void setFormatSep()
        {
            format = new System.Globalization.NumberFormatInfo();
            format.NumberDecimalSeparator = ",";

            string[] f = { "C", "C0", "C1", "C2", "C3",
                           "E", "E0", "E1", "E2", "E3", 
                           "F", "F0", "F1", "F2", "F3", 
                           "G", "G0", "G1", "G2", "G3", 
                           "N", "N0", "N1", "N2", "N3", 
                           "P", "P0", "P1", "P2", "P3", 
                           "R", "#,000.000", "0.###E-000", "000,000,000,000.00###"
                         };

            foreach (string s in f)
                toolStripComboBoxFormat.Items.Add(s);
        }

        zg.SymbolType pointView = new zg.SymbolType();
        void setComboBox()
        {
            string[] a = {"Круги", "Многоугольники" , "Плюсы", "Квадраты", "Звезды", "Треугольники вверх", "Треугольники вниз",
                             "V", "X", "По умолчанию", "Ничего"};
            comboBoxPoints.Items.AddRange(a);
            comboBoxPoints.Text = a[a.Length - 1];
            pointView = zg.SymbolType.None;

        }
        //проверка удобоваримости фи и ро
        bool provFiAndRo()
        {
            if (fi(0) == ro(0, 0))
            {
                if (fi(2 * h) == ro(1, 0))
                    return true;
            }
            return false;
        }

        void setCheckBox()
        {
            if (provFiAndRo())
            {
                labelSopr.Text = "Условие сопряжения выполнено";
                toolTip1.SetToolTip(labelSopr, fi(0).ToString() + " = " + ro(0, 0).ToString() + "\n" +
                    fi(2 * h).ToString() + " = " + ro(1, 0).ToString());
            }
            else
            {
                labelSopr.Text = "Условие сопряжения не выполнено";
                toolTip1.SetToolTip(labelSopr, fi(0).ToString() + " ?= " + ro(0, 0).ToString() + "\n" +
                    fi(2 * h).ToString() + " ?= " + ro(1, 0).ToString());
            }
        }

        int N = 0;
        int n = 0;
        double h = 0;
        double GAMMA = 0;
        double R = 0;
        double TAU = 0;
        double TETA = 0;
        double x = 0;
        double alpha = 0;
        double beta = 0;

        //найти в листках
        int N_
        {
            get { return Convert.ToInt32(textBoxN.Text, format); }
        }

        int n_
        {
            
            get {
                addVar("<b>n</b>", (N - 1).ToString());
                return (int)(N - 1); //соответствует записям
            }
        }
        //просто h
        double h__
        {
            get {
                double en = 0;
                if (textBoxh.Text != "")
                {
                    if (textBoxh.Text[textBoxh.Text.Length - 1] == ',')
                        en = Convert.ToDouble(textBoxh.Text.Replace(",", ""), format);
                    else
                        en = Convert.ToDouble(textBoxh.Text, format);
                }
                else
                    en = 1;
                return en; }
        }

        //h с индексом
        double h_(int j)
        {
            return j * h__;//соответствует записям
        }

        double R_
        {
            get
            {
                double one = 7 - 48 * TETA * TETA;//соответствует записям
                return one;
            }
        }

        double GAMMA_
        {
            get
            {
                double en = 0;
                if (textBoxG.Text != "")
                {
                    if (textBoxG.Text[textBoxG.Text.Length - 1] == ',')
                        en = Convert.ToDouble(textBoxG.Text.Replace(",", ""), format);
                    else
                        en = Convert.ToDouble(textBoxG.Text, format);
                }
                else
                    en = 1;
                return en;
            }
        }

        
        double TAU_//пишется как волнистое т
        {
            get {
                double f = 1;
                double s = 2 * N;
                double result = f / s;//соответствует записям
                addVar("<b>τ</b>", result.ToString());
                return result;
            }
        }

        List<string> listTau = new List<string>();
        //тау с индексом
        double tau(int i)
        {
            double result = i * TAU;//соответствует записям
            //addVar("<b>τ</b> с индексом " + i.ToString(), result.ToString());
            if (!isHave(listTau, "<b>τ с индексом " + i.ToString() + "</b> = " + result.ToString()))
                listTau.Add("<b>τ с индексом " + i.ToString() + "</b> = " + result.ToString());
            return result;
        }

        //просто тета
        double TETA_//ɵ
        {
            get {
                double result = (GAMMA * GAMMA * TAU / (h * h));//соответствует записям
                addVar("<b>ɵ</b>", result.ToString());
                return result;
            }
        }

        //x
        double x_
        {
            get {
                
                double result = (double)((R - 14) / R);//соответствует записям
                addVar("<b>x</b>", result.ToString());
                return result;
            }
        }

        //альфа
        double alpha_
        {
            get {
                double result = (double)((R - 14 - 32 * TETA) / R);//соответствует записям
                addVar("<b>α</b>", result.ToString());
                return result;
            }
        }

        //бета
        double beta_
        {
            get {
                double result = (double)((R - 14 + 32 * TETA) / R);//соответствует записям
                addVar("<b>β</b>", result.ToString());
                return result;
           }
        }

        List<PointF> ro0List = new List<PointF>();
        List<PointF> ro1List = new List<PointF>();
        //ро                                                                !!!!
        double ro(int index, double var)
        {
            double result;
            if (index == 0)
            {
                result = p.Evaluate(comboBoxRo0.Text, var);//Math.PI * var - Math.Sin(Math.PI * var);//
                if (!isHave(ro0List, new PointF((float)var, (float)result)))
                    ro0List.Add(new PointF((float)var, (float)result));
            }
            else
            {
                result = p.Evaluate(comboBoxRo1.Text, var); //2 * Math.PI * var - Math.Sin(2 * Math.PI * var);// 
                if (!isHave(ro1List, new PointF((float)var, (float)result)))
                    ro1List.Add(new PointF((float)var, (float)result));
            }
            
            addVar("<b>ρ</b>", result.ToString());
            return result;
        }

        List<PointF> fiList = new List<PointF>();
        //фи                                                                !!!!
        double fi(double var)
        {
            double result = p.Evaluate(comboBoxFi.Text, var);
            if (!isHave(fiList, new PointF((float)var, (float)result)))
                fiList.Add(new PointF((float)var, (float)result));
            addVar("<b>ϕ</b>", result.ToString());
            return result; 
        }

        //омега малая
        double omega(int k)
        {
            return arrayOmega[k];
        }

        double[] arrayOmega;
        List<string> listOmegaSmall = new List<string>();
        void getArrOmega(int lenght)
        {
            arrayOmega = new double[lenght + 1];
            for (int i = 1; i < arrayOmega.Length; i++)
            {
                double one = X(0, i);
                double two = X(1, i);
                double three = 4 * TETA;
                arrayOmega[i] = (one + two) / three;
                if (radioButtonOn.Checked)
                    listOmegaSmall.Add("<b>ω в степени " + i.ToString() + "</b> = " + arrayOmega[i].ToString());
            }
        }

        //омега большая
        double OMEGA(int k)
        {
            return arrayBigOmega[k];
        }

        double[] arrayBigOmega;
        List<string> listOmegaBig = new List<string>();
        void getArrBigOmega(int length)
        {
            arrayBigOmega = new double[length + 1];
            for (int k = 1; k <= length; k++)
            {
                if (radioButtonOn.Checked)
                    listOmegaBig.Add("Считаю <b>W в степени " + k.ToString() + "</b>");
                double first = (1 + alpha) * omega(k);
                if (radioButtonOn.Checked)
                {
                    listOmegaBig.Add("    Первое слагаемое = " + first.ToString());
                }
                double second = (1 + beta) * omega(k + 1);
                if (radioButtonOn.Checked)
                {
                    listOmegaBig.Add("    Второе слагаемое = " + second.ToString());
                }
                double third = first + second;//соответствует записям
                if (radioButtonOn.Checked)
                {
                    listOmegaBig.Add("<b>W в степени " + k.ToString() + "</b> = " + third.ToString());
                }
                arrayBigOmega[k] = third;
            }
        }

        //кси
        double ksi(int k)
        {
            return arrayKsi[k];
        }
        double[] arrayKsi;
        List<string> listKsi = new List<string>();
        void getArrKsi(int length)
        {
            arrayKsi = new double[length + 1];
            double first;
            double second;
            double third;
            first = fi(h_(2)) - 2 * fi(h_(1)) + fi(h_(0));
            if (radioButtonOn.Checked)
                if (!isHave(listKsi, "    Первое слагаемое = " + first.ToString()))
                    listKsi.Add("    Первое слагаемое = " + first.ToString());
            second = omega(1);
            if (radioButtonOn.Checked)
                if (!isHave(listKsi, "    Второе слагаемое = " + second.ToString()))
                    listKsi.Add("    Второе слагаемое = " + second.ToString());
            third = first - second;//соответствует записям
            if (radioButtonOn.Checked)
            {
                if (!isHave(listKsi, "<b>ξ в степени " + 0.ToString() + "</b> = " + third.ToString()))
                    listKsi.Add("<b>ξ в степени " + 0.ToString() + "</b> = " + third.ToString());
            }
            arrayKsi[0] = third;
            for (int k = 1; k <= length; k++)
            {
                first = omega(k);
                if (radioButtonOn.Checked)
                    if (!isHave(listKsi, "    Первое слагаемое = " + first.ToString()))
                        listKsi.Add("    Первое слагаемое = " + first.ToString());
                second = omega(k + 1);
                if (radioButtonOn.Checked)
                    if (!isHave(listKsi, "    Второе слагаемое = " + second.ToString()))
                        listKsi.Add("    Второе слагаемое = " + second.ToString());
                third = first - second;//соответствует записям
                if (radioButtonOn.Checked)
                {
                    if (!isHave(listKsi, "<b>ξ в степени " + k.ToString() + "</b> = " + third.ToString()))
                        listKsi.Add("<b>ξ в степени " + k.ToString() + "</b> = " + third.ToString());
                }
                arrayKsi[k] = third;
            }
        }

        //многочлен Чебышева
        double U(int k) // многочлен Чебышева
        {
            return arrayU[k];
        }
        
        double[] arrayU;
        void getArrU(int length)
        {
            arrayU = new double[length + 1];
            arrayU[0] = 1;
            arrayU[1] = 2 * x;
            for (int i = 2; i <= length; i++)
                arrayU[i] = ((2 * x * arrayU[i - 1] - arrayU[i - 2]));
        }

        //переопределенный многочлен Чебышева
        double[] arrayP;
        void getArrP(int length)
        {
            arrayP = new double[length + 1];
            arrayP[0] = 1;
            for (int i = 1; i <= length; i++)
                arrayP[i] = arrayU[i] - beta * arrayU[i - 1];
        }

        List<string> listP = new List<string>();
        //переопределенный многочлен Чебышева
        double P(int k)
        {
            if (radioButtonOn.Checked)
            {
                if (!isHave(listP, "Многочлен Чебышева <b>P</b> с индексом " + k.ToString() + " = " + arrayP[k].ToString()))
                    listP.Add("Многочлен Чебышева <b>P</b> с индексом " + k.ToString() + " = " + arrayP[k].ToString());
            }
            return arrayP[k];
        }

        
        //X
        double X(int index, int k)
        {
            double result;
            if (index == 0)
            {
                result = arrayX0[k];//соответствует записям
                listX.Add("<b>X c индексом " + index.ToString() + " в степени " + k.ToString() + "</b> = " + result.ToString());
                return result;
            }
            else if (index == 1)
            {
                result = arrayX1[k];//соответствует записям
                listX.Add("<b>X c индексом " + index.ToString() + " в степени " + k.ToString() + "</b> = " + result.ToString());
                return result;
            }
            if (radioButtonOn.Checked)
            addLog("Параметр <b>X</b> вычислен с ошибкой. Возвращено значение -1.");
            return -1;
        }
        List<string> listX = new List<string>();
        
        //Y
        double Y(int index, int k)
        {
            double result;
            if (index == 0)
            {
                result = arrayY0[k];//соответствует записям
                listY.Add("<b>Y c индексом " + index.ToString() + " в степени " + k.ToString() + "</b> = " + result.ToString());
                return result;
            }
            else if (index == 1)
            {
                result = arrayY1[k];//соответствует записям
                listY.Add("<b>Y c индексом " + index.ToString() + " в степени " + k.ToString() + "</b> = " + result.ToString());
                return result;
            }
            if (radioButtonOn.Checked)
                addLog("Параметр <b>Y</b> вычислен с ошибкой. Возвращено значение -1.");
            return -1;
        }
        List<string> listY = new List<string>();

        double[] arrayX0;
        double[] arrayX1;
        double[] arrayY0;
        double[] arrayY1;
        void getArrXY(int length)
        {
            arrayX0 = new double[length + 1];
            arrayX1 = new double[length + 1];
            arrayY0 = new double[length + 1];
            arrayY1 = new double[length + 1];
            for (int k = 0; k <= length; k++)
            {
                arrayX0[k] = ro(0, tau(2 * k)) - ro(0, tau(2 * k - 2));
                arrayX1[k] = ro(1, tau(2 * k)) - ro(1, tau(2 * k - 2));
                arrayY0[k] = ro(0, tau(2 * k)) - 2 * ro(0, tau(2 * k - 1)) + ro(0, tau(2 * k - 2));
                arrayY1[k] = ro(1, tau(2 * k)) - 2 * ro(1, tau(2 * k - 1)) + ro(1, tau(2 * k - 2));
            }
        }

        //z^2*k (6.2)
        double Z(int K)
        {
            return arrayZ[K];
        }

        List<string> listZ = new List<string>();
        double[] arrayZ;
        void getArrZ(int length)
        {
            arrayZ = new double[length + 1];
            arrayZ[0] = fi(h_(2)) - 2 * fi(h_(1)) + fi(h_(0));
            for (int K = 1; K <= length; K++)
            {
                int k = (int)K / 2;
                if (k != 0)
                {
                    if (radioButtonOn.Checked)
                        listZ.Add("Считаю <b>Z в степени " + (2 * k).ToString() + "</b>");
                    if (radioButtonOn.Checked)
                        if (!isHave(listZ, "    Первое слагаемое = " + omega(k).ToString()))
                            listZ.Add("    Первое слагаемое = " + omega(k).ToString());
                    double first = 0;
                    for (int i = 0; i <= k - 1; i++)
                    {
                        double minusOne = Math.Pow(-1, k + i);
                        first += minusOne * P(i) * ksi(i);
                    }
                    if (radioButtonOn.Checked)
                        if (!isHave(listZ, "    Ряд второго слагаемого = " + first.ToString()))
                            listZ.Add("    Ряд второго слагаемого = " + first.ToString());

                    double second = (double)(P(N - k) / P(N)) * first;
                    if (radioButtonOn.Checked)
                        if (!isHave(listZ, "    Второе слагаемое =" + second.ToString()))
                            listZ.Add("    Второе слагаемое =" + second.ToString());
                    double third = (1 - alpha * beta) * ((double)((U(k - 1)) / P(N)));
                    if (radioButtonOn.Checked)
                        if (!isHave(listZ, "    Множитель ряда третьего слагаемого = " + third.ToString()))
                            listZ.Add("    Множитель ряда третьего слагаемого = " + third.ToString());
                    double fourth = 0;
                    for (int i = k; i <= n; i++)
                    {
                        double minusOne = Math.Pow(-1, k + i);
                        fourth += minusOne * U(n - i) * ksi(i);
                    }
                    if (radioButtonOn.Checked)
                        if (!isHave(listZ, "    Ряд третьего слагаемого = " + fourth.ToString()))
                            listZ.Add("    Ряд третьего слагаемого = " + fourth.ToString());

                    double fifth = omega(k) + second + third * fourth;//соответствует записям
                    if (radioButtonOn.Checked)
                        if (!isHave(listZ, "<b>Z в степени " + (2 * k).ToString() + "</b>" + " = " + fifth.ToString()))
                            listZ.Add("<b>Z в степени " + (2 * k).ToString() + "</b>" + " = " + fifth.ToString());
                    arrayZ[2 * k] = fifth;
                }
            }
        }

        List<string> listBigTeta = new List<string>();
        //большая тета (7.4)
        double BIG_TETA
        {
            get {
                if (radioButtonOn.Checked)
                {
                    listBigTeta.Add("Считаю <b>θ</b>");
                }
                double first = (-beta) * Z(0) * Z(0);
                if (radioButtonOn.Checked)
                {
                    listBigTeta.Add("    Первое слагаемое = " + first.ToString());
                }
                double second = 2 * (1 + beta) * omega(1) * Z(0);
                if (radioButtonOn.Checked)
                {
                    listBigTeta.Add("    Второе слагаемое = " + second.ToString());
                }
                double third = 0;
                for (int k = 1; k <= N; k++)
                    third += omega(k) * omega(k);
                third *= 2 * (1 + x);
                if (radioButtonOn.Checked)
                {
                    listBigTeta.Add("    Третее слагаемое = " + third.ToString());
                }
                double fourth = Z(0) * Z(2);
                if (radioButtonOn.Checked)
                {
                    listBigTeta.Add("    Четвертое слагаемое = " + fourth.ToString());
                }
                double fifth = 0;
                for (int k = 1; k <= n; k++)
                    fifth += OMEGA(k) * Z(2 * k);
                if (radioButtonOn.Checked)
                {
                    listBigTeta.Add("    Пятое слагаемое = " + fifth.ToString());
                }
                double sixth = (1 + alpha) * omega(N) * Z(2 * N);
                if (radioButtonOn.Checked)
                {
                    listBigTeta.Add("    Шестое слагаемое = " + sixth.ToString());
                }
                double result = first + second - third - fourth + fifth + sixth;//соответствует записям
                if (radioButtonOn.Checked)
                {
                    listBigTeta.Add("<b>θ</b> = " + result.ToString());
                }
                return result;
            }
        }

        void clearAll()
        {
            arrayBigOmega = new double[0];
            arrayKsi = new double[0];
            arrayOmega = new double[0];
            arrayP = new double[0];
            arrayU = new double[0];
            arrayX0 = new double[0];
            arrayX1 = new double[0];
            arrayY0 = new double[0];
            arrayY1 = new double[0];
            arrayZ = new double[0];
            
        }

        //(7.3)
        double jStar
        {
            get {
                try
                {
                    N = Convert.ToInt32(textBoxN.Text, format); 
                    n = N - 1;
                    h = Convert.ToDouble(textBoxh.Text, format);
                    GAMMA = Convert.ToDouble(textBoxG.Text, format);
                    TAU = ((double)1 / (double)(2 * N));
                    TETA = GAMMA * GAMMA * TAU / (h * h);
                    R = 7 - 48 * TETA * TETA;
                    x = ((double)(R - 14) / (double)R);
                    alpha = ((double)(R - 14 - 32 * TETA) / (double)R);
                    beta = ((double)(R - 14 + 32 * TETA) / (double)R);
                    //в скобках - размерность массива
                    getArrXY(N);
                    getArrU(N);
                    getArrP(N);
                    getArrOmega(N);
                    getArrBigOmega(N - 1);
                    getArrKsi(N - 1);
                    getArrZ(2 * N);
                    if (radioButtonOn.Checked)
                        addLog("Считаю <b>J*</b>");
                    double first;
                    double second = 0;
                    double third = 0;
                    double fourth;
                    double fifth;
                    first = (3 * R * BIG_TETA);
                    if (radioButtonOn.Checked)
                        addLog("    Первое слагаемое", (first.ToString()));
                    labelsecond.Text = string.Format(sFormat, first.ToString());
                    for (int k = 1; k <= N; k++)
                        second += ( Math.Pow((X(0, k) - X(1, k)), 2));
                    second = 12 * second;
                    if (radioButtonOn.Checked)
                        addLog("    Второе слагаемое", second.ToString());
                    labelthird.Text = string.Format(sFormat, second.ToString());
                    for (int k = 1; k <= N; k++)
                    {
                        double one = 31 * Y(0, k) * Y(0, k);
                        double two = 2 * Y(0, k) * Y(1, k);
                        double three = 31 * Y(1, k) * Y(1, k);
                        third += (one + two + three);
                    }
                    if (radioButtonOn.Checked)
                        addLog("    Третее слагаемое", third.ToString());
                    labelfourth.Text = string.Format(sFormat, third.ToString());
                    fourth = first + second + third;//соответствует записям
                    if (radioButtonOn.Checked)
                    addLog("    Сумма трех слагаемых", fourth.ToString());
                    fifth = ((Convert.ToDouble(fourth) * h) / (144 * TAU));//соответствует записям
                    labelfirst.Text = string.Format(sFormat, ((144 * TAU) / h).ToString());
                    labelResult.Text = "J* = " + fifth.ToString();
                    if (radioButtonOn.Checked)
                        addLog("<b>Значение функционала J*</b>", fifth.ToString());
                    return fifth;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.StackTrace, "Проверьте синтаксис использованных тегов!");
                    return -1;
                }
            }
        }

        private void textBoxN_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox)
                if (((TextBox)sender).Text.IndexOf(".") != -1)
                {
                    if (((TextBox)sender).Text.IndexOf(",") != -1)
                        ((TextBox)sender).Text = ((TextBox)sender).Text.Replace(".", "");
                    else
                        ((TextBox)sender).Text = ((TextBox)sender).Text.Replace(".", ",");
                }
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
            //double doub = jStar;
        }

        void addLog(string caption, string value)
        {
            richTextBoxLog.Text += caption + " = " + value + "\n";
        }

        void addLog(string caption)
        {
            richTextBoxLog.Text += caption + "\n";
        }

        void addVar(string caption, string value)
        {
            if (radioButtonOn.Checked)
                if (richTextBoxVar.Text.IndexOf(caption) == -1)
                    richTextBoxVar.Text += caption + " = " + value + "\n";
        }

        //
        //оформление ричтекстбокса
        //
        string bold = "<b>";
        string boldE = "</b>";
        string italic = "<i>";
        string italicE = "</i>";
        string under = "<u>";
        string underE = "</u>";
        string middle = "<m>";
        string middleE = "</m>";
        string center = "<c>";
        string left = "<l>";
        string right = "<r>";
        string defaultText = "";

        class selections
        {
            //позиция начала выделения жирного
            public int selectionStart = 0;
            //позиция окончания выделения жирного
            public int selectionEnd = 0;

            //какое выделение
            public bool bold = false;
            public bool italic = false;
            public bool under = false;
            public bool middle = false;
            public bool center = false;
            public bool left = false;
            public bool right = false;
            //индекс строки выделения в массиве всех строк
            public int index = 0;
        }

        //каждая строка может иметь несколько выделений
        List<List<selections>> listListSelections = new List<List<selections>>();

        void selectText(RichTextBox richTextBox)
        {
            try
            {
                listListSelections.Clear();
                List<string> rtbList = new List<string>();
                for (int i = 0; i < richTextBox.Lines.Length; i++)
                {
                    rtbList.Add(richTextBox.Lines[i]);
                }
                for (int i = 0; i < rtbList.Count; i++)
                {
                    List<selections> listSelections = new List<selections>();
                    string s = rtbList[i];
                    string sWhisOnlyTag = "";
                    sWhisOnlyTag = deleteTags(s, bold, boldE);
                    while (sWhisOnlyTag.IndexOf(bold) != -1)
                    {
                        selections sel = new selections();
                        sel.bold = true;
                        int Start = sWhisOnlyTag.IndexOf(bold);
                        sWhisOnlyTag = sWhisOnlyTag.Remove(Start, bold.Length);
                        int End = sWhisOnlyTag.IndexOf(boldE);
                        sWhisOnlyTag = sWhisOnlyTag.Remove(End, boldE.Length);
                        if (End == -1)
                            End = Start;
                        sel.index = i;
                        sel.selectionStart = Start;
                        sel.selectionEnd = End;
                        listSelections.Add(sel);
                    }
                    sWhisOnlyTag = deleteTags(s, italic, italicE);
                    while (sWhisOnlyTag.IndexOf(italic) != -1)
                    {
                        selections sel = new selections();
                        sel.italic = true;
                        int Start = sWhisOnlyTag.IndexOf(italic);
                        sWhisOnlyTag = sWhisOnlyTag.Remove(Start, italic.Length);
                        int End = sWhisOnlyTag.IndexOf(italicE);
                        sWhisOnlyTag = sWhisOnlyTag.Remove(End, italicE.Length);
                        if (End == -1)
                            End = Start;
                        sel.index = i;
                        sel.selectionStart = Start;
                        sel.selectionEnd = End;
                        listSelections.Add(sel);
                    }
                    sWhisOnlyTag = deleteTags(s, under, underE);
                    while (sWhisOnlyTag.IndexOf(under) != -1)
                    {
                        selections sel = new selections();
                        sel.under = true;
                        int Start = sWhisOnlyTag.IndexOf(under);
                        sWhisOnlyTag = sWhisOnlyTag.Remove(Start, under.Length);
                        int End = sWhisOnlyTag.IndexOf(underE);
                        sWhisOnlyTag = sWhisOnlyTag.Remove(End, underE.Length);
                        if (End == -1)
                            End = Start;
                        sel.index = i;
                        sel.selectionStart = Start;
                        sel.selectionEnd = End;
                        listSelections.Add(sel);
                    }
                    sWhisOnlyTag = deleteTags(s, middle, middleE);
                    while (sWhisOnlyTag.IndexOf(middle) != -1)
                    {
                        selections sel = new selections();
                        sel.middle = true;
                        int Start = sWhisOnlyTag.IndexOf(middle);
                        sWhisOnlyTag = sWhisOnlyTag.Remove(Start, middle.Length);
                        int End = sWhisOnlyTag.IndexOf(middleE);
                        sWhisOnlyTag = sWhisOnlyTag.Remove(End, middleE.Length);
                        if (End == -1)
                            End = Start;
                        sel.index = i;
                        sel.selectionStart = Start;
                        sel.selectionEnd = End;
                        listSelections.Add(sel);
                    }
                    if (s.IndexOf(center) != -1)
                    {
                        selections sel = new selections();
                        sel.center = true;
                        sel.index = i;
                        listSelections.Add(sel);
                    }
                    if (s.IndexOf(left) != -1)
                    {
                        selections sel = new selections();
                        sel.left = true;
                        sel.index = i;
                        listSelections.Add(sel);
                    }
                    if (s.IndexOf(right) != -1)
                    {
                        selections sel = new selections();
                        sel.right = true;
                        sel.index = i;
                        listSelections.Add(sel);
                    }
                    listListSelections.Add(listSelections);
                    rtbList[i] = deleteTags(s);

                }
                richTextBox.Text = "";
                int length = 0;
                for (int i = 0; i < rtbList.Count; i++)
                {
                    richTextBox.AppendText(rtbList[i] + "\n");
                    for (int j = 0; j < listListSelections[i].Count; j++)
                    {
                        int end = Math.Abs(listListSelections[i][j].selectionEnd - listListSelections[i][j].selectionStart);
                        if (i != 0)
                            richTextBox.Select(listListSelections[i][j].selectionStart + length, end);
                        else
                            richTextBox.Select(listListSelections[i][j].selectionStart, Math.Abs(listListSelections[i][j].selectionEnd - listListSelections[i][j].selectionStart));
                        if (listListSelections[i][j].bold)
                        {
                            richTextBox.SelectionFont = new System.Drawing.Font(richTextBox.Font, FontStyle.Bold);
                        }
                        if (listListSelections[i][j].italic)
                        {
                            richTextBox.SelectionFont = new System.Drawing.Font(richTextBox.Font, FontStyle.Italic);
                        }
                        if (listListSelections[i][j].under)
                        {
                            richTextBox.SelectionFont = new System.Drawing.Font(richTextBox.Font, FontStyle.Underline);
                        }
                        if (listListSelections[i][j].middle)
                        {
                            richTextBox.SelectionFont = new System.Drawing.Font(richTextBox.Font, FontStyle.Strikeout);
                        }
                        if (listListSelections[i][j].center)
                            richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                        if (listListSelections[i][j].left)
                            richTextBox.SelectionAlignment = HorizontalAlignment.Left;
                        if (listListSelections[i][j].right)
                            richTextBox.SelectionAlignment = HorizontalAlignment.Right;
                    }
                    length += rtbList[i].Length + 1;
                    int lrtb = richTextBox.Text.Length;
                }
                deleteTags(richTextBox);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Проверьте синтаксис использованных тегов!");

            }
        }

        void deleteTags(RichTextBox richTextBox)
        {
            string[] tags = { bold, boldE, italic, italicE, under, underE, middle, middleE, center, left, right };
            foreach (string tag in tags)
                richTextBox.Text.Replace(tag, "");
        }

        string deleteTags(string s)
        {
            string[] tags = { bold, boldE, italic, italicE, under, underE, middle, middleE, center, left, right };
            foreach (string tag in tags)
                s = s.Replace(tag, "");
            return s;
        }

        //удаляет все теги, кроме двух указаных
        string deleteTags(string s, string exceptionTag1, string exceptionTag2)
        {
            string[] tags = { bold, boldE, italic, italicE, under, underE, middle, middleE, center, left, right };
            foreach (string tag in tags)
                if (exceptionTag1 != tag && exceptionTag2 != tag)
                    s = s.Replace(tag, "");
            return s;
        }

        List<string> deleteTags(List<string> l)
        {
            for (int i = 0; i < l.Count; i++ )
                l[i] = deleteTags(l[i]);
            return l;
        }

        private void groupBoxresult_Enter(object sender, EventArgs e)
        {

        }

        //
        //закончили оформлять ричтекстбокс
        //

        void mySort(string[] a)
        {

        }

        bool goodFunc()
        {
            bool ok = false;
            double var1 = p.Evaluate(comboBoxRo0.Text, 1);
            ok = notErrors(p);
            if (!ok)
                return false;
            double var2 = p.Evaluate(comboBoxRo1.Text, 1);
            ok = notErrors(p);
            if (!ok)
                return false;
            double var3 = p.Evaluate(comboBoxFi.Text, 1);
            ok = notErrors(p);
            if (!ok)
                return false;
            return ok;
        }

        bool notErrors(Parser p)
        {
            if (p.err == Parser.Errors.NOEXP)
            {    MessageBox.Show("Ошибка: Выражение(-я) отсутствует. \nЗамечание: введите функцию(-и)."); return false; }
            if (p.err == Parser.Errors.OTHER)
            {    MessageBox.Show("Ошибка: Нарушение алгоритма. \nЗамечание: обратитесь к разработчику."); return false; }
            if (p.err == Parser.Errors.SYNTAX)
            {    MessageBox.Show("Ошибка: Синтаксическая ошибка. \nЗамечание: проверьте корректность введённой(-ых) функции(-й)."); return false; }
            if (p.err == Parser.Errors.UNBALPARENTS)
            {    MessageBox.Show("Ошибка: Дисбаланс скобок. \nЗамечание: Количество открывающих скобок не равно количеству закрывающих скобок."); return false; }
            if (p.err == Parser.Errors.DIVBYZERO)
            {    MessageBox.Show("Ошибка: Деление на нуль."); return false; }
            if (p.err == Parser.Errors.BREAK)
            { MessageBox.Show("Ошибка: Разрыв."); return false; }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (goodFunc())
            {
                clearAll();
                DateTime time1 = new DateTime();
                DateTime time2 = new DateTime();
                time1 = DateTime.Now;
                //очищаем списки
                object[] arr = { N, n, h, GAMMA, TAU, TETA, x, listTau, listX, listY, ro0List, ro1List, fiList, listKsi, listOmegaSmall, listOmegaBig, listBigTeta, listZ };
                foreach (object o in arr)
                    if (o is List<double>)
                        ((List<double>)o).Clear();
                    else if (o is List<string>)
                        ((List<string>)o).Clear();

                if (!isHave(listFi, comboBoxFi.Text))
                {
                    listFi.Add(comboBoxFi.Text);
                    comboBoxFi.Items.Add(comboBoxFi.Text);
                }
                if (!isHave(listRo0, comboBoxRo0.Text))
                {
                    listRo0.Add(comboBoxRo0.Text);
                    comboBoxRo0.Items.Add(comboBoxRo0.Text);
                }
                if (!isHave(listRo1, comboBoxRo1.Text))
                {
                    listRo1.Add(comboBoxRo1.Text);
                    comboBoxRo1.Items.Add(comboBoxRo1.Text);
                }
                lBegin.Text = "Начало вычисления: ";
                lEnd.Text = "Конец вычисления: ";
                lTime.Text = "Затрачено времени: ";

                richTextBoxLog.Clear();
                richTextBoxVar.Clear();
                richTextBoxLog.Visible = richTextBoxVar.Visible = false;
                labelWait.Visible = !radioButtonOn.Checked;
                

                double doub = jStar;
                labelWait.Text = "J* = " + string.Format(sFormat, doub.ToString()); 
                setTable();

                if (radioButtonOn.Checked)
                {
                    richTextBoxLog.Visible = richTextBoxVar.Visible = true;
                    selectText(richTextBoxLog);
                    selectText(richTextBoxVar);
                }
                time2 = DateTime.Now;
                lBegin.Text += time1.ToLongTimeString();
                lEnd.Text += time2.ToLongTimeString();
                lTime.Text += (time2 - time1).ToString();

                textBoxN.Focus();
            }
        }

        private void radioButtonOff_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOn.Checked)
                radioButtonOff.Checked = false;
            else if (radioButtonOff.Checked)
                radioButtonOn.Checked = false;
        }

        private void keyPress(object sender, KeyPressEventArgs e)
        {
            if (sender is RichTextBox)
            {
                if (e.KeyChar == '-')
                {
                    ((RichTextBox)sender).Font = new Font(((RichTextBox)sender).Font.Name, (float)(((RichTextBox)sender).Font.Size - 0.5), FontStyle.Regular);
                }
                else if (e.KeyChar == '+')
                    ((RichTextBox)sender).Font = new Font(((RichTextBox)sender).Font.Name, (float)(((RichTextBox)sender).Font.Size + 0.5), FontStyle.Regular);
            }
        }

        private void labelfirst_MouseHover(object sender, EventArgs e)
        {
            if (sender is Label)
            {
                if (((Label)sender).Name.IndexOf("first") != -1)
                {
                    toolTip1.SetToolTip(((Label)sender), labelfirst.Text);
                }
                else if (((Label)sender).Name.IndexOf("second") != -1)
                {
                    toolTip1.SetToolTip(((Label)sender), labelsecond.Text);
                }
                else if (((Label)sender).Name.IndexOf("third") != -1)
                {
                    toolTip1.SetToolTip(((Label)sender), labelthird.Text);
                }
                else if (((Label)sender).Name.IndexOf("fourth") != -1)
                {
                    toolTip1.SetToolTip(((Label)sender), labelfourth.Text);
                }
                else if (((Label)sender).Name.IndexOf("Result") != -1)
                {
                    toolTip1.SetToolTip(((Label)sender), labelResult.Text);
                } 
            }
        }

        int currentW = 0;
        int oldW = 0;
        private void GeneralForm_Resize(object sender, EventArgs e)
        {
            
            //currentW = Width;
            ////свободное пространство
            //int width = Math.Abs(currentW - oldW);//buttonGo.Location.X - (textBoxRo.Location.X + textBoxRo.Width);
            //int w1 = width / 3;
            //int w2 = width - w1;

            
            //if (currentW > oldW)
            //{
            //    if (richTextBoxLog.Location.X + richTextBoxLog.Width < Width - 10)
            //    {
            //        richTextBoxVar.Width += w1;
            //        richTextBoxLog.Location = new Point(richTextBoxLog.Location.X + w1, richTextBoxLog.Location.Y);
            //        richTextBoxLog.Width += w2;
            //    }
            //}
            //else if (currentW < oldW)
            //{
            //    if (richTextBoxLog.Location.X + richTextBoxLog.Width > MinimumSize.Width)
            //    {
            //        richTextBoxVar.Width -= w1;
            //        richTextBoxLog.Location = new Point(richTextBoxLog.Location.X - w1, richTextBoxLog.Location.Y);
            //        richTextBoxLog.Width -= w2;
            //    }
            //}
            
            ////if (richTextBoxLog.Location.X + richTextBoxLog.Width > buttonGo.Location.X - 6)
            //    //richTextBoxLog.Size = new System.Drawing.Size(buttonGo.Location.X - 6, richTextBoxLog.Height);

            //oldW = currentW;
        }

        

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox)
            {
                if (((CheckBox)sender).Checked)
                    ((CheckBox)sender).Text = "Условие сопряжения выполнено";
                else
                    ((CheckBox)sender).Text = "Условие сопряжения НЕ выполнено";
            }
        }

        private void показатьЗначенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(getValuesFi(), "ϕ");
            //MessageBox.Show(getValuesRo(), "ρ");
            //char[] sep = { '\n' };
            //string[] s = getValuesFi().Split(sep);  
            //listBox1.Items.Add("                                 ϕ");
            //listBox1.Items.Add(DateTime.Now.ToLongTimeString());
            //listBox1.Items.AddRange(s);
            //string[] t = getValuesRo().Split(sep);
            //listBox2.Items.Add("                                 ρ");
            //listBox2.Items.Add(DateTime.Now.ToLongTimeString());
            //listBox2.Items.AddRange(s);
            //panelShow.Visible = true;
            MessageBox.Show(prUsl, "Проверка условий");
        }

        void myIncInt(TextBox tb)
        {
            int i = Convert.ToInt32(tb.Text);
            i += 1;
            tb.Text = i.ToString();
        }

        void myIncFloat(TextBox tb)
        {
            double i = Convert.ToDouble(tb.Text);
            i += 0.1 ;
            tb.Text = i.ToString();
        }

        void myDecInt(TextBox tb)
        {
            int i = Convert.ToInt32(tb.Text);
            i -= 1;
            tb.Text = i.ToString();
        }

        void myDecFloat(TextBox tb)
        {
            double i = Convert.ToDouble(tb.Text);
            i -= 0.1;
            tb.Text = i.ToString();
        }

        private void textBoxN_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox)
            {
                if (e.KeyCode == Keys.Down)
                {
                    switch (((TextBox)sender).Name)
                    {
                        case "textBoxN": myDecInt(((TextBox)sender));
                            break;
                        case "textBoxG": myDecFloat(((TextBox)sender));
                            break;
                        case "textBoxh": myDecInt(((TextBox)sender));
                            break;
                    }
                }
                else if (e.KeyCode == Keys.Up)
                {
                    switch (((TextBox)sender).Name)
                    {
                        case "textBoxN": myIncInt(((TextBox)sender));
                            break;
                        case "textBoxG": myIncFloat(((TextBox)sender));
                            break;
                        case "textBoxh": myIncFloat(((TextBox)sender));
                            break;
                    }
                }
            }
        }

        private void textBoxRo0_Leave(object sender, EventArgs e)
        {
            
        }

        private void textBoxRo0_MouseEnter(object sender, EventArgs e)
        {
            if (sender is TextBox)
                toolTip1.SetToolTip(((TextBox)sender), ((TextBox)sender).Text);
        }


        List<string> listRo0 = new List<string>();
        List<string> listRo1 = new List<string>();
        List<string> listFi = new List<string>();
        //записывает в файл все уравнения
        void writeFileEq()
        {
            //readFileEq();
            StreamWriter sr = new StreamWriter(fileEq);
            sr.WriteLine("#ro0");
            foreach(string s in listRo0)
                sr.WriteLine(s);
            sr.WriteLine("#ro1");
            foreach (string s in listRo1)
                sr.WriteLine(s);
            sr.WriteLine("#fi");
            foreach (string s in listFi)
                sr.WriteLine(s);
            sr.Close();
        }
        //считывает из файла все уравнения
        void readFileEq()
        {
            string[] arr = File.ReadAllLines(fileEq, myEnc);
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].IndexOf("#ro0") != -1)
                    while (i < arr.Length && arr[i].IndexOf("#ro1") == -1 && arr[i].IndexOf("#fi") == -1)
                    {
                        if (arr[i].IndexOf("#ro0") == -1)
                            listRo0.Add(arr[i]);

                        i++;
                    }
                if (arr[i].IndexOf("#ro1") != -1)
                    while (i < arr.Length && arr[i].IndexOf("#fi") == -1 )
                    {
                        if (arr[i].IndexOf("#ro1") == -1)
                            listRo1.Add(arr[i]);
                        i++;
                    }
                if (arr[i].IndexOf("#fi") != -1)
                    while (i < arr.Length)
                    {
                        if (arr[i].IndexOf("#fi") == -1)
                            listFi.Add(arr[i]);
                        i++;
                    }
            }
        }

        private void GeneralForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            writeFileEq();
        }

        //есть ли уже в этом списке данная строка
        bool isHave(List<string> l, string s)
        {
            foreach (string ss in l)
                if (ss == s)
                    return true;
            return false;
        }

        bool isHave(List<double> l, double s)
        {
            foreach (double ss in l)
                if (ss == s)
                    return true;
            return false;
        }

        bool isHave(List<PointF> l, PointF s)
        {
            foreach (PointF ss in l)
                if (ss.X == s.X && ss.Y == s.Y)
                    return true;
            return false;
        }

        int arrLenght = 0;
        //заполняем таблицу логом
        void setTable()
        {
            try
            {
                N = N_;
                n = n_;
                h = h__;
                GAMMA = GAMMA_;
                R = R_;
                TAU = TAU_;
                TETA = TETA_;
                x = x_;
                alpha = alpha_;
                beta = beta_;
                object[] arr = { N, n, h, GAMMA, TAU, TETA, R, x, alpha, beta, listTau, listX, listY, ro0List, ro1List, fiList, listP, listKsi, listOmegaSmall, listOmegaBig, listBigTeta, listZ };
                arrLenght = arr.Length;
                groupBoxVar.Controls.Clear();
                int labelInterval = 5;
                int labelWidth = 0;
                for (int i = 0; i < arrLenght; i++)
                {
                    RichTextBox rtb = new RichTextBox();
                    rtb.Name = "rtb" + i.ToString();
                    rtb.Visible = false;

                    if (arr[i] is List<double>)
                    {
                        foreach (double o in ((List<double>)arr[i]))
                        {
                            rtb.Text += ((double)o).ToString() + "\n";
                        }
                        if (rtb.Text.Length != 0)
                            rtb.Text = rtb.Text.Substring(0, rtb.Text.Length - 1);
                    }
                    else if (arr[i] is List<string>)
                    {
                        foreach (string o in ((List<string>)arr[i]))
                        {
                            rtb.Text += ((string)o) + "\n";
                        }
                        if (rtb.Text.Length != 0)
                            rtb.Text = rtb.Text.Substring(0, rtb.Text.Length - 1);
                    }
                    else if (arr[i] is List<PointF>)
                    {
                        foreach (PointF o in ((List<PointF>)arr[i]))
                        {
                            rtb.Text += "x: " + ((PointF)o).X + "; y: " + ((PointF)o).Y + "\n";
                        }
                        if (rtb.Text.Length != 0)
                            rtb.Text = rtb.Text.Substring(0, rtb.Text.Length - 1);
                    }
                    else if (arr[i] is double)
                        rtb.Text += ((double)arr[i]);

                    Label lbl = new Label();
                    lbl.Name = "lbl" + i.ToString();
                    lbl.Anchor = AnchorStyles.Left;
                    lbl.Location = new Point(labelWidth + labelInterval, 18);
                    //labelInterval = 30;
                    lbl.Size = new System.Drawing.Size(30, 15);
                    labelWidth += lbl.Width;

                    lbl.MouseEnter += new EventHandler(lbl_MouseEnter);
                    lbl.Click += new EventHandler(lbl_Click);
                    switch (i)
                    {
                        case 0:
                            {
                                lbl.Text = "N";
                                rtb.Text = N.ToString();
                            }
                            break;
                        case 1:
                            {
                                lbl.Text = "n";
                                rtb.Text = n.ToString();
                            }
                            break;
                        case 2:
                            {
                                lbl.Text = "h";
                                rtb.Text = h.ToString();
                            }
                            break;
                        case 3:
                            {
                                lbl.Text = "γ";
                                rtb.Text = GAMMA.ToString();
                            }
                            break;
                        case 4:
                            {
                                lbl.Text = "τ";
                                rtb.Text = TAU.ToString();
                            }
                            break;
                        case 5:
                            {
                                lbl.Text = "ɵ";
                                rtb.Text = TETA.ToString();
                            }
                            break;
                        case 6:
                            {
                                lbl.Text = "R";
                                rtb.Text = R.ToString();
                            }
                            break;
                        case 7:
                            {
                                lbl.Text = "x";
                                rtb.Text = x.ToString();
                            }
                            break;
                        case 8:
                            {
                                lbl.Text = "α";
                                rtb.Text = alpha.ToString();
                            }
                            break;
                        case 9:
                            {
                                lbl.Text = "β";
                                rtb.Text = beta.ToString();
                            }
                            break;
                        case 10: lbl.Text = "τ[i]";
                            break;
                        case 11: lbl.Text = "X";
                            break;
                        case 12: lbl.Text = "Y";
                            break;
                        case 13: lbl.Text = "ρ0";
                            break;
                        case 14: lbl.Text = "ρ1";
                            break;
                        case 15: lbl.Text = "ϕ";
                            break;
                        case 16: lbl.Text = "P";
                            break;
                        case 17: lbl.Text = "ξ";
                            break;
                        case 18: lbl.Text = "ω";
                            break;
                        case 19: lbl.Text = "W";
                            break;
                        case 20: lbl.Text = "θ";
                            break;
                        case 21: lbl.Text = "Z^2k";
                            break;
                    }
                    groupBoxVar.Controls.Add(lbl);
                    groupBoxVar.Controls.Add(rtb);


                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace + "\n" + e.Message);
            }
        }

        void lbl_Click(object sender, EventArgs e)
        {
            if (sender is Label)
                for (int i = 0; i < arrLenght; i++)
                    if (((Label)sender).Name.IndexOf(i.ToString()) != -1)
                        foreach (object o in groupBoxVar.Controls)
                            if (o is RichTextBox)
                                if (((RichTextBox)o).Name == "rtb" + i.ToString())
                                {

                                    richTextBoxLog.Text = "<b>" + ((Label)sender).Text + "</b> \n" + ((RichTextBox)o).Text;
                                    selectText(richTextBoxLog);
                                }
        }

        void lbl_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Label)
                for (int i = 0; i < arrLenght; i++)
                    if (((Label)sender).Name.IndexOf(i.ToString()) != -1)
                        foreach (object o in groupBoxVar.Controls)
                            if (o is RichTextBox)
                                if (((RichTextBox)o).Name == "rtb" + i.ToString())
                                    if ("τ[i]XYρ0ρ1ϕPξωWZ^2k".IndexOf(((Label)sender).Text) == -1 || ((Label)sender).Text == "τ")
                                    { toolTip1.SetToolTip(((Label)sender), deleteTags(((RichTextBox)o).Text)); }
                                    else
                                    { 
                                        string s = "";
                                        for (int j = 0; j < 5; j++)
                                        {
                                            if (((RichTextBox)o).Lines.Length > j)
                                                s += deleteTags(((RichTextBox)o).Lines[j]) + "\n";
                                        }
                                        if (s.IndexOf("...") == -1)
                                            s += "...";
                                        toolTip1.SetToolTip(((Label)sender), s);
                                    }
        }

        //для построение графика
        List<PointF> allJ = new List<PointF>();
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                allJ.Clear();
                DateTime time1 = new DateTime();
                DateTime time2 = new DateTime();
                time1 = DateTime.Now;
                lBegin.Text = "Начало вычисления: ";
                lEnd.Text = "Конец вычисления: ";
                lBegin.Text += time1.ToLongTimeString();
                radioButtonOff.Checked = true;
                List<string> l = new List<string>();
                l.Add("ρ0 = " + comboBoxRo0.Text);
                l.Add("ρ1 = " + comboBoxRo1.Text);
                l.Add("ϕ = " + comboBoxFi.Text);
                if (nToolStripMenuItem.Checked)
                {
                    l.Add("γ = " + GAMMA.ToString());
                    l.Add("h = " + h.ToString());
                    int begin = 0;
                    int end = 0;
                    char[] sep = { ';' };
                    string[] sArr = toolStripTextBoxInterval.Text.Split(sep);
                    begin = Convert.ToInt32(sArr[0].Trim());
                    end = Convert.ToInt32(sArr[1].Trim());
                    int inc = Convert.ToInt32(toolStripTextBoxStep.Text.Trim());
                    double must = Math.Sqrt((double)12 / (double)7) * (GAMMA * GAMMA / (h * h));
                    while (begin <= must)
                        begin++;
                    for (int i = begin; i <= end; i += inc)
                    {
                        //очищаем списки
                        //object[] arr = { N, n, h, GAMMA, TAU, TETA, x, listTau, listX, listY, ro0List, ro1List, fiList, listP, listKsi, listOmegaSmall, listOmegaBig, listBigTeta, listZ };
                        //foreach (object o in arr)
                        //    if (o is List<double>)
                        //        ((List<double>)o).Clear();
                        //    else if (o is List<string>)
                        //        ((List<string>)o).Clear();

                        textBoxN.Text = i.ToString();
                        //decimal j = jStar;
                        double fr = fastResult();
                        PointF p = new PointF((float)N_, (float)fr);
                        allJ.Add(p);
                        provUsl();
                        if (печататьПроверкуУсловийToolStripMenuItem.Checked)
                        {
                            char[] sp = { '\n' };
                            string[] sA = prUsl.Split(sp);
                            foreach (string s in sA)
                                l.Add(s);
                        }
                        l.Add("N = " + i.ToString() + " J* = " + fr.ToString());
                    }
                }
                else
                {
                    l.Add("N = " + N.ToString());
                    double begin = 0;
                    double end = 0;
                    char[] sep = { ';' };
                    string[] sArr = toolStripTextBoxInterval.Text.Split(sep);
                    begin = Convert.ToDouble(sArr[0].Trim());
                    end = Convert.ToDouble(sArr[1].Trim());
                    double inc = Convert.ToDouble(toolStripTextBoxStep.Text.Trim());
                    if (hToolStripMenuItem.Checked)
                    {
                        l.Add("γ = " + GAMMA.ToString());
                        
                        for (double i = begin; i <= end; i += inc)
                        {
                            //очищаем списки
                            //object[] arr = { N, n, h, GAMMA, TAU, TETA, x, listTau, listX, listY, ro0List, ro1List, fiList, listP, listKsi, listOmegaSmall, listOmegaBig, listBigTeta, listZ };
                            //foreach (object o in arr)
                            //    if (o is List<double>)
                            //        ((List<double>)o).Clear();
                            //    else if (o is List<string>)
                            //        ((List<string>)o).Clear();

                            textBoxh.Text = i.ToString();
                            provUsl();
                            //decimal j = jStar;
                            double fr = fastResult();
                            l.Add("#");
                            if (печататьПроверкуУсловийToolStripMenuItem.Checked)
                            {
                                char[] sp = { '\n' };
                                string[] sA = prUsl.Split(sp);
                                foreach (string s in sA)
                                    l.Add(s);
                            }
                            l.Add("h = " + i.ToString() + " J* = " + fr.ToString());
                        }
                    }
                    else
                    {
                        l.Add("h = " + h.ToString());
                        for (double i = begin; i <= end; i += inc)
                        {
                            //очищаем списки
                            //object[] arr = { N, n, h, GAMMA, TAU, TETA, x, listTau, listX, listY, ro0List, ro1List, fiList, listP, listKsi, listOmegaSmall, listOmegaBig, listBigTeta, listZ };
                            //foreach (object o in arr)
                            //    if (o is List<double>)
                            //        ((List<double>)o).Clear();
                            //    else if (o is List<string>)
                            //        ((List<string>)o).Clear();

                            textBoxG.Text = i.ToString();
                            //decimal j = jStar;
                            double fr = fastResult();
                            provUsl();
                            l.Add("#");
                            if (печататьПроверкуУсловийToolStripMenuItem.Checked)
                            {
                                char[] sp = { '\n' };
                                string[] sA = prUsl.Split(sp);
                                foreach (string s in sA)
                                    l.Add(s);
                            }
                            l.Add("γ = " + i.ToString() + " J* = " + fr.ToString());
                        }
                    }
                }
                time2 = DateTime.Now;
                lEnd.Text += time2.ToLongTimeString();
                lTime.Text = "Затраченное временя: " + (time2 - time1).ToString();
                l.Add("Затраченное время: " + lTime.Text);
                MySave(l);
                
                //File.WriteAllLines(Application.StartupPath + "//log//" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Millisecond.ToString() + ".txt", l, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void comboBoxFi_Leave(object sender, EventArgs e)
        {
            setCheckBox();
            
        }

        private void labelSopr_MouseEnter(object sender, EventArgs e)
        {
            setCheckBox();
        }
        string prUsl = "";
        //проверка всех условий
        void provUsl()
        {
            double nWould = Math.Sqrt((double)((double)12 / (double)7)) * ((double)(GAMMA * GAMMA) / (double)(h * h));
            bool uslN = false;
            if (n > nWould)
                uslN = true;

            bool uslAlphaBeta1 = false;
            if (alpha + beta == 2 * x)
                uslAlphaBeta1 = true;

            bool uslAlphaBeta2 = false;
            if (alpha * beta > 1)
                uslAlphaBeta2 = true;

            bool uslAlphaXBeta = false;
            double a = (1 / beta);
            double c = a + beta;
            double b = 2;
            a = c / b;
            if (alpha < x && x < a && a <= -1)
                uslAlphaXBeta = true;

            bool uslXBeta = false;
            if (x < beta && beta < 0)
                uslXBeta = true;


            bool uslTeta = false;
            if (TETA * TETA < (double)((double)7 / (double)48))
                uslTeta = true;
            string s = "";
            string ok = "Условие выполнено.";
            string notOk = "Условие не выполнено.";
            s = "Проверка условия N > sqrt(12/7)*((γ*γ)/(h*h))\n";
            if (uslN)
                s += ok + "\n\n";
            else
                s += notOk + " N = " + N.ToString() + " ?> " + nWould.ToString() + "\n\n";

            s += "Проверка условия ɵ * ɵ < 7/48 \n";
            if (uslTeta)
                s += ok + "\n\n";
            else
                s += notOk + "ɵ * ɵ = " + (TETA * TETA).ToString() + " ?< " + ((double)((double)7 / (double)48)).ToString() + "\n Следовательно R <= 0\n";

            s += "Проверка условия α + β = 2x\n";
            if (uslAlphaBeta1)
                s += ok + "\n\n";
            else
                s += notOk + " α + β = " + (alpha + beta).ToString() + "\n\n";
            s += "Проверка условия αβ > 1\n";
            if (uslAlphaBeta2)
                s += ok + "\n\n";
            else
                s += notOk + " αβ = " + (alpha * beta).ToString() + "\n\n";
            s += "Проверка условия α < x < (β^(-1) + β)/2 <= -1\n";
            if (uslAlphaXBeta)
                s += ok + "\n\n";
            else
                s += notOk +
                    " α = " + alpha.ToString() + " ?< " + 
                    "x = " + x.ToString() + " ?< " + 
                    "(β^(-1) + β)/2 = " + a.ToString() + " ?<= 1" 
                    + "\n\n";
            s += "Проверка условия x < β < 0\n";
            if (uslXBeta)
                s += ok + "\n\n";
            else
                s += notOk +
                    " x = " + x.ToString() + " ?< " +
                    "β = " + beta.ToString() + " ?< 0" + 
                    "\n\n";
            prUsl = s;
            if (uslN && uslAlphaBeta1 && uslAlphaBeta2 && uslAlphaXBeta && uslXBeta)
            {
                buttonGo.Enabled = true;
            }
            else
            {
                //MessageBox.Show(prUsl, "Некоторые условия не выполнены");
                //buttonGo.Enabled = false;
            }
            
        }

        private void textBoxG_Leave(object sender, EventArgs e)
        {
            provUsl();
        }

        private void toolStripComboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripComboBox)
                if (((ToolStripComboBox)sender).Text != "")
                    formatInfo = ((ToolStripComboBox)sender).Text;
        }

        private void nToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ((ToolStripMenuItem)sender).Checked = true;
                if (((ToolStripMenuItem)sender).Name == "nToolStripMenuItem")
                {
                    hToolStripMenuItem.Checked = false;
                    gToolStripMenuItem.Checked = false;
                }
                else if (((ToolStripMenuItem)sender).Name == "hToolStripMenuItem")
                {
                    nToolStripMenuItem.Checked = false;
                    gToolStripMenuItem.Checked = false;
                }
                else if (((ToolStripMenuItem)sender).Name == "gToolStripMenuItem")
                {
                    nToolStripMenuItem.Checked = false;
                    hToolStripMenuItem.Checked = false;
                }
            }

        }

        private void textBoxN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                if (sender is TextBox)
                {
                    if (((TextBox)sender).Name == "textBoxN" || ((TextBox)sender).Name == "tbChoose")
                    {
                        if (e.KeyChar != '\b')
                        {
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        if (e.KeyChar != ',' && e.KeyChar != '\b')
                        {
                            e.Handled = true;
                        }
                    }
                }
                else if (sender is ToolStripTextBox)
                {
                    if (e.KeyChar != ',' && e.KeyChar != '\b')
                    {
                        e.Handled = true;
                    }
                }

            }
        }


        //сохранение в txt
        void MySave(List<string> l)
        {
            try
            {
                //filename = Application.StartupPath + "\\mySavedFiles\\" + ".txt";
                SaveFileDialog savedialog = saveFileDialog1;
                savedialog.FileName = DateTime.Now.ToShortTimeString().Replace(":", "-") + "_" + DateTime.Now.ToShortDateString().Replace(".", "-") + "_";
                savedialog.Title = "Сохранить как ...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter =
                   "Текстовые файлы(*.txt)|*.txt|Все файлы(*.*)|*.*";
                savedialog.ShowHelp = true;
                char[] sep = { '\\' };
                // If selected, save
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the user-selected file name
                    string fileName = savedialog.FileName;
                    //fileName = fileName.Remove(fileName.Length - 4);
                    // Get the extension
                    string strFilExtn =
                        fileName.Remove(0, fileName.Length - 3);
                    // Save file
                    switch (strFilExtn)
                    {
                        case "txt":
                            {
                                File.WriteAllLines(fileName, l, myEnc);
                                lTime.Text += " Результаты сохранены в файл: " + fileName;
                            }
                            break;
                    }
                    
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        }

        private void печататьПроверкуУсловийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            richTextBoxLog.Text = richTextBoxVar.Text = "";
            double result = fastResult();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFunc();
        }

        void saveFunc()
        {
            try
            {
                SaveFileDialog savedialog = saveFileDialog1;
                savedialog.FileName = "N=" + N.ToString() + "h=" + h.ToString() + "gamma=" + GAMMA.ToString();
                savedialog.Title = "Сохранить как ...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter =
                   "Текстовые файлы(*.txt)|*.txt|Все файлы(*.*)|*.*";
                savedialog.ShowHelp = true;
                char[] sep = { '\\' };
                // If selected, save
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the user-selected file name
                    string fileName = savedialog.FileName;
                    //fileName = fileName.Remove(fileName.Length - 4);
                    // Get the extension
                    string strFilExtn =
                        fileName.Remove(0, fileName.Length - 3);
                    // Save file
                    switch (strFilExtn)
                    {
                        case "txt":
                            {

                                File.WriteAllLines(fileName, saveAll(), myEnc);
                                lTime.Text += " Результаты сохранены в файл: " + fileName;
                                richTextBoxLog.Text += "Результаты сохранены в файл: " + fileName;
                            }
                            break;
                    }

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        
        }

        List<string> saveAll()
        {
            List<string> l = new List<string>();
            //object[] arr = { N, n, h, GAMMA, TAU, TETA, x, listTau, listX, listY, ro0List, ro1List, fiList, listU, listP, listKsi, listOmegaSmall, listOmegaBig, listBigTeta, listZ };
            
            l.Add(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            l.Add("N = " + N.ToString());
            l.Add("n = " + n.ToString());
            l.Add("h = " + h.ToString());
            l.Add("γ = " + GAMMA.ToString());
            l.Add("τ = " + TAU.ToString());
            l.Add("ɵ = " + TETA.ToString());
            l.Add("x = " + x.ToString());
            if (listTau.Count != 0)
            {
                l.Add("Список значений τ с индексами:");
                foreach (string s in listTau)
                    l.Add("    " + s);
            }
            if (listX.Count != 0)
            {
                l.Add("Список значений X:");
                foreach (string s in listX)
                    l.Add("    " + s);
            }
            if (listY.Count != 0)
            {
                l.Add("Список значений Y:");
                foreach (string s in listY)
                    l.Add("    " + s);
            }
            if (ro0List.Count != 0)
            {
                l.Add("Список значений ρ0: ");
                foreach (PointF s in ro0List)
                    l.Add("    " + s.X + ":" + s.Y);
            }
            if (ro1List.Count != 0)
            {
                l.Add("Список значений ρ1: ");
                foreach (PointF s in ro1List)
                    l.Add("    " + s.X + ":" + s.Y);
            }
            if (fiList.Count != 0)
            {
                l.Add("Список значений ϕ: ");
                foreach (PointF s in fiList)
                    l.Add("    " + s.X + ":" + s.Y);
            }
            if (arrayU != null)
            {
                l.Add("Список значений многочлена Чебышева:");
                for (int i = 0; i < arrayU.Length; i++)
                    l.Add("    U(" + i.ToString() + ") = " + arrayU[i].ToString());
            }
            if (listP.Count != 0)
            {
                l.Add("Список значений переопределенного многочлена Чебышева:");
                foreach (string s in listP)
                    l.Add("    " + s.ToString());
            }
            if (listKsi.Count != 0)
            {
                l.Add("Список значений ξ:");
                foreach (string s in listKsi)
                    l.Add("    " + s);
            }
            if (listOmegaSmall.Count != 0)
            {
                l.Add("Список значений ω:");
                foreach (string s in listOmegaSmall)
                    l.Add("    " + s);
            }
            if (listOmegaBig.Count != 0)
            {
                l.Add("Список значений W:");
                foreach (string s in listOmegaBig)
                    l.Add("    " + s);
            }
            if (listBigTeta.Count != 0)
            {
                l.Add("Список значений θ:");
                foreach (string s in listBigTeta)
                    l.Add("    " + s);
            }
            if (listZ.Count != 0)
            {
                l.Add("Список значений Z^2k:");
                foreach (string s in listZ)
                    l.Add("    " + s);
            }
            l.Add("Первое слагаемое = " + labelsecond.Text);
            l.Add("Второе слагаемое = " + labelthird.Text);
            l.Add("Третее слагаемое = " + labelfourth.Text);
            l.Add("Функционал " + labelResult.Text);
            l.Add(lTime.Text);
            l.Add("");
            return deleteTags(l);
        }

        List<double> zArr = new List<double>();
        //вычисление функционала без записей логов итп
        double fastResult()
        {
            try
            {
                zArr.Clear();
                DateTime time1 = new DateTime();
                DateTime time2 = new DateTime();
                time1 = DateTime.Now;
                int N = Convert.ToInt32(textBoxN.Text, format);
                int n = N - 1;
                double h = Convert.ToDouble(textBoxh.Text, format);
                double gamma = Convert.ToDouble(textBoxG.Text, format);
                double tau = ((double)1 / (double)(2 * N));
                double theta = gamma * gamma * tau / (h * h);
                double _R = 7 - 48 * theta * theta; ;
                double x = ((double)(_R - 14) / (double)_R);
                double a = ((double)(_R - 14 - 32 * theta) / (double)_R);
                double b = ((double)(_R - 14 + 32 * theta) / (double)_R);
                double z2k = 0;
                double S = 0;
                double first = 0;
                double second = 0;
                double third = 0;
                double J = -1;

                //double[,] u = new double[2 * N + 1, 3];
                double[] z = new double[2 * N + 1];
                double[] X_0 = new double[N + 1];
                double[] X_1 = new double[N + 1];
                double[] Y_0 = new double[N + 1];
                double[] Y_1 = new double[N + 1];
                double[] omega = new double[N + 1];
                double[] W = new double[N - 1 + 1];
                double[] xi = new double[N - 1 + 1];
                double[] U = new double[N + 1];
                double[] P = new double[N + 1];

                if (_R <= 0)
                {
                    double must = Math.Sqrt(12.0 / 7.0) * Math.Pow(gamma / h, 2.0);
                    MessageBox.Show("Ошибка: R = " + _R.ToString() + "должна быть <= 0. N =" + N.ToString() + " должна быть > " + must.ToString());
                }
                else
                {
                    //многочлен Чебышева
                    U[0] = 1;
                    U[1] = 2 * x;
                    for (int i = 2; i <= N; i++)
                        U[i] = 2 * x * U[i - 1] - U[i - 2];
                    //переопределенный многочлен Чебышева
                    P[0] = 1;
                    for (int i = 1; i <= N; i++)
                        P[i] = U[i] - b * U[i - 1];
                    //матрица u
                    //for (int i = 0; i <= 2; i++)
                    //    u[0, i] = fi(i * h);
                    //for (int i = 0; i <= 2 * N; i++)
                    //    u[i, 0] = ro(0, i * tau);
                    //for (int i = 0; i <= 2 * N; i++)
                    //    u[i, 2] = ro(1, i * tau);

                    //z[0] = u[0, 2] - 2 * u[0, 1] + u[0, 0];
                    z[0] = fi(h_(2)) - 2 * fi(h_(1)) + fi(h_(0));
                    zArr.Add(z[0]);
                    //X и Y
                    for (int i = 1; i <= N; i++)
                    {
                        X_0[i] = ro(0, 2 * i * tau) - ro(0, (2 * i - 2) * tau);//u[2 * i, 0] - u[2 * i - 2, 0];
                        X_1[i] = ro(1, 2 * i * tau) - ro(1, (2 * i - 2) * tau);//u[2 * i, 2] - u[2 * i - 2, 2];
                        Y_0[i] = ro(0, 2 * i * tau) - 2 * ro(0, (2 * i - 1) * tau) + ro(0, (2 * i - 2) * tau);//u[2 * i, 0] - 2 * u[2 * i - 1, 0] + u[2 * i - 2, 0];
                        Y_1[i] = ro(1, 2 * i * tau) - 2 * ro(1, (2 * i - 1) * tau) + ro(1, (2 * i - 2) * tau);//u[2 * i, 2] - 2 * u[2 * i - 1, 2] + u[2 * i - 2, 2];
                    }
                    //омега малая
                    for (int i = 1; i <= N; i++)
                    {
                        omega[i] = (double)(X_0[i] + X_1[i]) / (4 * theta);
                    }
                    //омега большая
                    for (int i = 1; i <= n; i++)
                        W[i] = (1 + a) * omega[i] + (1 + b) * omega[i + 1];
                    //кси
                    xi[0] = z[0] - omega[1];
                    for (int i = 1; i <= n; i++)
                        xi[i] = omega[i] - omega[i + 1];
                    //z2k
                    for (int i = 1; i <= N; i++)
                    {
                        z2k = omega[i];
                        S = 0;
                        for (int k = 0; k <= i - 1; k++)
                        {
                            S += Math.Pow(-1, i + k) * P[k] * xi[k];
                        }
                        z2k += S * P[N - i] / P[N];
                        S = 0;
                        for (int k = i; k <= n; k++)
                        {
                            S += Math.Pow(-1, i + k) * U[n - k] * xi[k];
                        }
                        z2k += (1 - a * b) * S * U[i - 1] / P[N];
                        z[2 * i] = z2k;
                        zArr.Add(z2k);
                    }
                    //первое слагаемое
                    S = 0;
                    for (int i = 1; i <= N; i++)
                        S += omega[i] * omega[i];
                    S = -b * z[0] * z[0] + 2 * (1 + b) * omega[1] * z[0] - 2 * (1 + x) * S - z[0] * z[2] + (1 + a) * omega[N] * z[2 * N];
                    for (int i = 1; i <= n; i++)
                        S += W[i] * z[2 * i];
                    first = 3 * _R * S;
                    //второе слагаемое
                    S = 0;
                    for (int i = 1; i <= N; i++)
                        S += Math.Pow(X_0[i] - X_1[i], 2.0);
                    S *= 12;
                    second = S;
                    //третье слагаемое
                    S = 0;
                    for (int i = 1; i <= N; i++)
                        S += 31 * Math.Pow(Y_0[i], 2) + 2 * Y_0[i] * Y_1[i] + 31 * Math.Pow(Y_1[i], 2);
                    third = S;
                    J = (first + second + third) / ((144 * tau) / h);
                    labelfirst.Text = ((144 * tau) / h).ToString();
                    labelsecond.Text = string.Format(sFormat, first.ToString());
                    labelthird.Text = string.Format(sFormat, second.ToString());
                    labelfourth.Text = string.Format(sFormat, third.ToString());
                    labelWait.Text = labelResult.Text = "J* = " + string.Format(sFormat, J.ToString());
                    
                    
                }
                return J;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
                return -1;
            }
        }

        float sqr(float x)
        {
            return x * x;
        }

        List<PointF> buildParab(PointF p1, PointF p2, PointF p3)
        {
            try
            {
                float x1 = p1.X;
                float x2 = p2.X;
                float x3 = p3.X;
                float y1 = p1.Y;
                float y2 = p2.Y;
                float y3 = p3.Y;

                float a = 0;
                float b = 0;
                float c = 0;
                //ищем уравнение параболы
                if (x2 - x1 != 0)
                {
                    a = (y3 - ((x3 * (y2 - y1) + x2 * y1 - x1 * y2) / (x2 - x1))) 
                        / (x3 * (x3 - x1 - x2) + x1 * x2);
                    b = (y2 - y1) / (x2 - x1) - a*(x1 + x2);
                    c = (x2 * y1 - x1 * y2) / (x2 - x1) + a * x1 * x2;

                }
                else if (p1.X == 0 && p2.X != 0 && p3.X != 0)
                { 
                    c = p1.Y;
                    a = (p3.Y + p1.Y * (p3.X - 1) - p2.Y * p3.X) / (sqr(p3.X) * p2.X - sqr(p2.X) * p3.X);
                    b = (p2.Y - p1.Y - a * sqr(p2.X)) / p2.X;
                }
                else if (p1.X != 0 && p2.X == 0 && p3.X != 0)
                {
                    c = p2.Y;
                    a = (- ((p1.Y - p2.Y) * p3.X - (p2.Y - p3.Y) * p1.X)) / (sqr(p3.X) * p1.X - sqr(p1.X) * p3.X);
                    b = (p1.Y - p2.Y - a * sqr(p1.X)) / p1.X;
                }
                else if (p1.X != 0 && p2.X != 0 && p3.X == 0)
                {
                    c = p3.Y;
                    a = (-((p2.Y - p3.Y) * p1.X - (p3.Y - p1.Y) * p2.X)) / (sqr(p1.X) * p2.X - sqr(p2.X) * p1.X);
                    b = (p2.Y - p3.Y - a * sqr(p2.X)) / p2.X;

                }
                
                List<PointF> list = new List<PointF>();
                List<string> log = new List<string>();
                float tau = ((float)1 / (float)(2 * N));
                if (a != 0 && b != 0 && c != 0)
                for (float i = 0; i < 7; i += tau)
                {
                    sqrEquation eq = new sqrEquation();
                    eq.calc(a, b, c - i);
                    if (!eq.notRealRoots)
                    {
                        if (eq.X1 >= h_(0) && eq.X1 <= h_(2))
                        { PointF point1 = new PointF(eq.X1, i); list.Add(point1); }
                        if (eq.X2 >= h_(0) && eq.X2 <= h_(2))
                        { PointF point2 = new PointF(eq.X2, i); list.Add(point2); }
                    }
                }
                list.Sort(new MyClassComparer());
                return list;
                
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
                return null;
            }
            

        }

        public class MyClassComparer : IComparer<PointF>
        {
            public int Compare(PointF p1, PointF p2)
            {
                return p1.X == p2.X ? 0 :
                    (p1.X < p2.X ? -1 : 1);
            }
        }

        //вычисление корней квадратного уравнения
        class sqrEquation
        {
            double x1 = 0;
            double x2 = 0;
            double y1 = 0;
            double y2 = 0;
            //нет действительных корней
            public bool notRealRoots = false;

            public void calc(double a, double b, double c)
            {
                //считаем дискриминант
                double D = sqr(b) - 4 * a * c;
                if (D > 0)
                {
                    x1 = (-b + Math.Sqrt(D)) / (2 * a);
                    y1 = a * sqr(x1) + b * x1 + c;
                    x2 = (-b - Math.Sqrt(D)) / (2 * a);
                    y2 = a * sqr(x2) + b * x2 + c;
                }
                else if (D == 0)
                {
                    if (a != 0)
                    {
                        x1 = x2 = (-b) / (2 * a);
                        y1 = y2 = a * sqr(x1) + b * x1 + c;
                    }
                    else
                        y1 = y2 = c;
                }
                else
                    notRealRoots = true;
            }

            public float X1
            {
                get { return (float)x1; }
            }

            public float X2
            {
                get { return (float)x2; }
            }

            public float Y1
            {
                get { return (float)y1; }
            }

            public float Y2
            {
                get { return (float)y2; }
            }

            double sqr(double x)
            {
                return x * x;
            }

            static public int MyClassCompareDate(float x1, float x2)
            {
                return x1.CompareTo(x2);
            }
        }

        private void построитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime time1 = DateTime.Now;
            Graph g = new Graph();
            List<List<PointF>> l = new List<List<PointF>>();
            int inc  = Convert.ToInt32(tbChoose.Text);
            for (int i = 0; i < zArr.Count; i += inc)
            {
                l.Add(buildParab(new PointF(0, (float)ro(0, i * TAU_)), 
                                new PointF((float)h, (float)zArr[i]),
                                new PointF((float)(2 * h), (float)ro(1, i * TAU_))));
            }
            g.DrawGraph(l, pointView, h_(2));
            DateTime time2 = DateTime.Now;
            g.Text = "Начало вычисления: " + time1.ToLongTimeString() + ". ";
            g.Text += "Конец вычисления: " + time2.ToLongTimeString() + ". ";
            g.Text += "Время построения: " + (time2 - time1).ToString();
            g.Show();
        }

        private void пробноеПостроениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graph g = new Graph();
            List<List<PointF>> l = new List<List<PointF>>();
            l.Add(buildParab(new PointF(0, 4), new PointF(1, 0), new PointF(2, -2)));
            g.DrawGraph(l, pointView, h_(2));
            g.Show();
        }

        private void графикПрогонкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graph g = new Graph();
            List<List<PointF>> l = new List<List<PointF>>();
            l.Add(allJ);
            g.DrawGraph(l, pointView, h_(2));
            g.Show();
        }

        private void comboBoxPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxPoints.SelectedIndex)
            {
                case 0: { pointView = zg.SymbolType.Circle; }
                    break;
                case 1: { pointView = zg.SymbolType.Diamond; }
                    break;
                case 2: { pointView = zg.SymbolType.Plus; }
                    break;
                case 3: { pointView = zg.SymbolType.Square; }
                    break;
                case 4: { pointView = zg.SymbolType.Star; }
                    break;
                case 5: { pointView = zg.SymbolType.Triangle; }
                    break;
                case 6: { pointView = zg.SymbolType.TriangleDown; }
                    break;
                case 7: { pointView = zg.SymbolType.VDash; }
                    break;
                case 8: { pointView = zg.SymbolType.XCross; }
                    break;
                case 9: { pointView = zg.SymbolType.Default; }
                    break;
                case 10: { pointView = zg.SymbolType.None; }
                    break;
                default: { pointView = zg.SymbolType.None; }
                    break;
            }
        }



        //

        //

        //

        //

        //

        //

        //

        //

        //

        //

        //

        //

        //

    }
}
