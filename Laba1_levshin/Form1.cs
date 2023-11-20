using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Laba1_levshin.Form1;

namespace Laba1_levshin
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        //public List<Itm> mod;
        public Comand[] comand1;
        public Comand[] comand2;

        #region consts
        public const int Tsh = 3;
        public const int Fo = 5;
        #endregion

        #region модель
        // 0 - черта 1 - декодировка 2 - вычесление 3 - упровление 4 - кеш
        public struct Comand
        {
            public int t;
            public bool kh;
            public int type;   // 0 - черта 1 - декодировка 2 - вычесление 3 - упровление 4 - кеш
            public int nomer;
            public int time;

            public Comand(int t, bool kh, int type)
            {
                this.t = t;
                this.kh = kh;
                this.type = type;
                nomer = 0;
                time = 0;
            }
            public Comand(int t, bool kh)
            {
                this.t = t;
                this.kh = kh;
                this.type = 0;
                nomer = 0;
                time = 0;
            }
        }

        public struct Itm
        {
            public int nomer;
            public int time;
            public int type;
            public int T;
            public Itm(int nomer, int time, int type, int T)
            {
                this.nomer = nomer;
                this.time = time;
                this.type = type;
                this.T = T;
            }
        }

        public partial class K1
        {
            public int timeStop;
            public int zadacha;



            public K1()
            {
                timeStop = 0;
                zadacha = 0;
            }


        }

        public partial class KK
        {
            public int timeStop;

            public KK()
            {
                timeStop = 0;
            }



        }

        public partial class Model
        {
            KK kk = new KK();
            K1 k1 = new K1();

            List<Itm> queueKK = new List<Itm>();
            List<Itm> queueK1 = new List<Itm>();

            Comand[] cs;

            public Model(Comand[] cs) { this.cs = cs; }

            public int Rabot()
            {
                
                int time = 0;

                int countCom = 0;

                //Один цыкл == Один такт

                while (true)
                {
                    // Если конвеир свободен и нет заявок от кэш контролера
                    // Обрабатывается новая команда
                    if (k1.timeStop <= 0 & queueK1.Count == 0)
                    {
                        // countCom - номер команды
                        // проверка того чтобы countCom не был больше количества команд
                        // Что означает что все команды обработаны
                        if (countCom == cs.Length)
                        {
                            //Доп проверка того что выполнены команды из КК
                            if (queueK1.Count == 0 & queueKK.Count == 0)
                            {
                                return time;
                            }
                        }
                        else
                        {
                            // Выполняется Декодировка

                            if (cs[countCom].kh)  // Данные есть в кэше
                            {
                                // Кэшь занят на 1 цикл
                                k1.timeStop = 1;

                                queueK1.Insert(0, new Itm(countCom, time, cs[countCom].type, cs[countCom].t));


                            }
                            else // Кэш промах
                            {

                                // Отпрака запроса в КК
                                queueKK.Add(new Itm(countCom, time, cs[countCom].type, cs[countCom].t));

                                countCom++;
                                // continue чтобы не защитало такт
                                continue;
                            }
                            countCom++;

                        }
                    }

                    // К1 свободен и есть заявка на работу
                    if (k1.timeStop <= 0 & queueK1.Count != 0)
                    {
                        // type == уровление устройством 
                        if (queueK1[0].type == 3)
                        {
                            // КК должен быть свободен 
                            // Иначе Должен ждать 
                            if (kk.timeStop <= 0)
                            {
                                Itm cur = queueK1[0];

                                k1.timeStop = Tsh * cur.T;
                                k1.zadacha = 3;

                                queueK1.RemoveAt(0);
                            }
                        }
                        else
                        {
                            if (queueK1[0].type == 2)
                            {

                                Itm cur = queueK1[0];

                                queueK1.RemoveAt(0);

                                k1.timeStop = 1 * cur.T;
                            }
                            else
                            {
                                if (queueK1[0].type == 1)
                                {
                                    k1.timeStop = 1;

                                    queueK1.RemoveAt(0);
                                }
                            }
                        }
                    }
                    // КК свободен и есть запрос
                    if (kk.timeStop <= 0 & queueKK.Count != 0 & k1.zadacha != 3)
                    {
                        kk.timeStop = Tsh * Fo;
                    }

                    if (kk.timeStop - 1 == 0)
                    {
                        Itm cur = queueKK[0];

                        cur.time = time;

                        queueK1.Add(new Itm(cur.nomer, time, 1, 1));
                        queueK1.Add(cur);

                        queueKK.RemoveAt(0);
                    }

                    if (k1.timeStop - 1 == 0)
                    {
                        k1.zadacha = 0;
                    }

                    // Условный такт
                    kk.timeStop--;
                    k1.timeStop--;

                    time++;

                }



            }
        }


        #endregion
        #region интерфейс


        private void button4_Click(object sender, EventArgs e)
        {

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            Random rnd = new Random();

            for (int i = 1; i <= 1000; i += 10)
            {

                comand1 = new Comand[i];
                comand2 = new Comand[i];


                ////(int t, bool kh, int type)

                for (int n = 0; n < i; n++)
                {
                    int t = 0;
                    bool kh = false;
                    int type = 0;

                    int rn = rnd.Next(0, 100);

                    if (rn >= 0 & rn < 20)
                    {
                        type = 2;

                        int rn2 = rnd.Next(0, 100);

                        if (rn2 >= 0 & rn2 < 70)
                            t = 5;
                        if (rn2 >= 70 & rn2 < 90)
                            t = 2;
                        if (rn2 >= 90 & rn2 < 100)
                            t = 1;

                    }
                    if (rn >= 20 & rn < 35)
                    {
                        type = 2;

                        int rn2 = rnd.Next(0, 100);

                        if (rn2 >= 0 & rn2 < 70)
                            t = 2;
                        if (rn2 >= 70 & rn2 < 90)
                            t = 5;
                        if (rn2 >= 90 & rn2 < 100)
                            t = 1;

                    }
                    if (rn >= 35 & rn < 50)
                    {
                        type = 3;

                        int rn2 = rnd.Next(0, 100);

                        if (rn2 >= 0 & rn2 < 80)
                            t = 2;
                        if (rn2 >= 80 & rn2 < 100)
                            t = 1;
                    }
                    if (rn >= 50 & rn < 100)
                    {
                        type = 2;

                        int rn2 = rnd.Next(0, 100);

                        if (rn2 >= 0 & rn2 < 60)
                            t = 2;
                        if (rn2 >= 60 & rn2 < 100)
                            t = 1;
                    }

                    rn = rnd.Next(0, 100);

                    if (rn < 75)
                    {
                        kh = true;
                    }
                    else
                        kh = false;

                    comand1[n] = new Comand(t, kh, type);

                    if (rn < 90)
                    {
                        kh = true;
                    }
                    else
                        kh = false;

                    comand2[n] = new Comand(t, kh, type);

                }

                Model m1 = new Model(comand1);
                Model m2 = new Model(comand2);

                chart1.Series[0].Points.AddXY(i, m1.Rabot());
                chart1.Series[1].Points.AddXY(i, m2.Rabot());

            }
        }
        #endregion
    }
}

  

