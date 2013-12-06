using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace OptAproxSplanSimpleHeatEquation
{
    public partial class Graph : Form
    {
        public Graph()
        {
            InitializeComponent();
        }


        public void DrawGraph(List<List<PointF>> l, SymbolType st, double H)
        {
            DateTime d1 = DateTime.Now;
            // Получим панель для рисования
            GraphPane pane = zedGraph.GraphPane;
            pane.XAxis.Title.Text = "h";
            pane.YAxis.Title.Text = "tau";
            pane.Title.Text = "Демонстрация поверхности";
            pane.XAxis.Scale.Min = 0;
            pane.XAxis.Scale.Max = H;
            pane.YAxis.Scale.Min = 0;
            pane.YAxis.Scale.Max = 1;
            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            //pane.CurveList.Clear();
            for (int i = 0; i < l.Count; i++)
            {
                // Создадим список точек
                PointPairList list = new PointPairList();

                // Заполняем список точек
                foreach (PointF p in l[i])
                {
                    // добавим в список точку
                    list.Add(p.X, p.Y);
                }

                // Создадим кривую с названием "Sinc", 
                // которая будет рисоваться голубым цветом (Color.Blue),
                // Опорные точки выделяться не будут (SymbolType.None)
                LineItem myCurve = pane.AddCurve("Парабола " + (i+1).ToString(), list, MyGraphColor, st);
                // отображать ли трубки в легенде
                myCurve.Label.IsVisible = false;
                // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
                // В противном случае на рисунке будет показана только часть графика, 
                // которая умещается в интервалы по осям, установленные по умолчанию
                
            }
            
            //PointPairList listRo0 = new PointPairList();
            //PointPairList listRo1 = new PointPairList();
            //for (float i = 0; i < 1; i += (float)0.01)
            //{
            //    listRo0.Add(0, i);
            //    listRo1.Add(H, i);
            //}

            //LineItem myCurve1 = pane.AddCurve("ρ0 ", listRo0, Color.Red, SymbolType.Square);
            //LineItem myCurve2 = pane.AddCurve("ρ1 ", listRo1, Color.Red, SymbolType.Square);
            zedGraph.AxisChange();
            zedGraph.Invalidate();
            DateTime d2 = DateTime.Now;
            Text = "Время построения графика: " + (d2 - d1);
            
        }

        Color MyGraphColor
        {
            get
            {
                Color[] MyColor = new Color[] {Color.Blue, Color.Navy, Color.BlueViolet, Color.CadetBlue, Color.CornflowerBlue, Color.Chartreuse, Color.DarkBlue,
                    Color.DarkSlateBlue, Color.DodgerBlue, Color.Indigo, Color.MediumBlue, Color.MidnightBlue, Color.RoyalBlue, Color.Blue};
                Random rand = new Random();
                int n = rand.Next(MyColor.Length);
                return MyColor[n];
            }
        }
    }
}
