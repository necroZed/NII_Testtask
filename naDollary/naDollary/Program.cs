using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace naDollary
{
    class Program
    {
        static void Main(string[] args)
        {
            string vvod, temp_str = "", op_type;
            int[] skobki = new int[2];
            string[] kurs = new string[2];
            string[] value1 = new string[2];
            string[] value2 = new string[2];
            kurs = Get_kurs();
            kurs[0] = kurs[0].Remove(0, 9);
            kurs[1] = kurs[1].Remove(0, 7);
            vvod = Console.ReadLine();
            vvod = vvod.Replace(".", ",");
            // toDollar(toEuro($10.00)+5eur)

            skobki = Obj_Find(vvod);
            while ((skobki[0] != 0) && (skobki[1] != 0))
            {
                temp_str = Get_Temp_String(vvod, skobki[0], skobki[1]);
                //Console.WriteLine("Выражение в скобках: {0}", temp_str);
                op_type = Wh_Money(vvod, skobki[0]);
                //Console.WriteLine("Тип перевода: {0}", op_type);
                if ((((temp_str.Contains("+")) || (temp_str.Contains("-"))) && (temp_str.LastIndexOf('-') > 2)) || ((temp_str.Contains("+")) && ((temp_str.LastIndexOf('-') < 2))))
                {
                    while ((((temp_str.Contains("+")) || (temp_str.Contains("-"))) && (temp_str.LastIndexOf('-') > 2)) || ((temp_str.Contains("+")) && ((temp_str.LastIndexOf('-') < 2))))
                    {
                        value1 = Get_value(temp_str);
                        //Console.WriteLine("Значение 1:{0}{1}", value1[0], value1[1]);
                        char i = temp_str[value1[0].Length + value1[1].Length];
                        //Console.WriteLine("Необходимая операция:{0}", i);
                        if (i == '+')
                        {
                            temp_str = temp_str.Remove(0, value1[0].Length + value1[1].Length + 1);
                            value2 = Get_value(temp_str);
                            //Console.WriteLine("Значение 2:{0}{1}", value2[0], value2[1]);
                            temp_str = temp_str.Remove(0, value2[0].Length + value2[1].Length);
                            value1 = Value_Plus(value1, value2);
                            if (value1[0] == "$")
                            {
                                temp_str = temp_str.Insert(0, value1[0] + value1[1]);
                            }
                            else temp_str = temp_str.Insert(0, value1[1] + value1[0]);
                            if (value1[0] != value2[0]) temp_str = "";
                        }
                        if (i == '-')
                        {
                            temp_str = temp_str.Remove(0, value1[0].Length + value1[1].Length + 1);
                            value2 = Get_value(temp_str);
                            //Console.WriteLine("Значение 2:{0}{1}", value2[0], value2[1]);
                            temp_str = temp_str.Remove(0, value2[0].Length + value2[1].Length);
                            value1 = Value_Minus(value1, value2);
                            if (value1[0] == "$")
                            {
                                temp_str = temp_str.Insert(0, value1[0] + value1[1]);
                            }
                            else temp_str = temp_str.Insert(0, value1[1] + value1[0]);
                            if (value1[0] != value2[0]) temp_str = "";
                        }
                        //Console.WriteLine("Результат операции: {0}", temp_str);
                    }
                }
                else
                {
                    value1 = Get_value(temp_str);
                    //Console.WriteLine("Значение:{0}{1}", value1[0], value1[1]);
                }
                if (temp_str != "")
                {
                    temp_str = Value_Convert(value1, kurs, op_type);
                    //Console.WriteLine("Результат перевода: {0}", temp_str);
                }
                vvod = vvod.Remove(skobki[0] - op_type.Length, skobki[1] - skobki[0] + 1 + op_type.Length);
                skobki = Obj_Find(vvod);
                if (vvod != "")
                {
                    vvod = vvod.Insert(skobki[0] + 1, temp_str);
                    //Console.WriteLine(vvod);
                    skobki = Obj_Find(vvod);
                }
                else
                {
                    vvod = temp_str;
                    if (temp_str != "")
                    {
                        value1[1] = string.Format("{0:f}", Convert.ToDouble(value1[1]));
                        if (value1[0] == "$") vvod = value1[0] + value1[1];
                        else vvod = value1[1] + value1[0];
                    }
                    vvod = vvod.Replace(",", ".");
                    Console.WriteLine(vvod);
                    break;
                }
            }

            Console.ReadKey();
        }

        public static int[] Obj_Find(string stroka)
        {
            int start = 0, finish = 0;

            for (int i = 0; i < stroka.Length; i++)
            {
                // Console.WriteLine("Счётчик {0}", i);

                if (stroka[i] == '(')
                {
                    start = i;
                    //Console.WriteLine("Старт {0}", start);
                }
                if (stroka[i] == ')')
                {
                    finish = i;
                    //Console.WriteLine("Финиш {0}", finish);
                    break;
                }
            }
            int[] qwerty = new int[2];
            qwerty[0] = start;
            qwerty[1] = finish;
            return qwerty;
        }

        public static string Get_Temp_String(string main_str, int start, int finish)
        {
            main_str = main_str.Remove(finish, (main_str.Length - finish));
            main_str = main_str.Remove(0, (start + 1));
            return main_str;
        }

        public static string Wh_Money(string stroka, int start)
        {
            string temp, new_temp = "", final = "";
            if (start != 0 && start >= 5)
            {
                temp = stroka.Substring(start - 6);
                new_temp = temp.Remove(6, temp.Length - 6);
                //Console.WriteLine(temp);
                //Console.WriteLine(new_temp);
                if (new_temp == "toEuro")
                {
                    final = new_temp;
                }
                else
                {
                    temp = stroka.Substring(start - 8);
                    new_temp = temp.Remove(8, temp.Length - 8);
                    //Console.WriteLine(temp);
                    //Console.WriteLine(new_temp);
                    if (new_temp == "toDollar")
                    {
                        final = new_temp;
                    }
                }
            }
            return final;
        }

        public static string[] Get_kurs()
        {
            string[] Mass = File.ReadAllLines(@"kurs.txt", System.Text.Encoding.Default);
            return Mass;
        }

        public static string[] Get_value(string stroka)
        {
            if (stroka.Contains("+"))
                stroka = stroka.Remove(stroka.IndexOf('+'));
            if ((stroka.Contains("-")) && (stroka[0] != '-') && (stroka[1] != '-'))
                stroka = stroka.Remove(stroka.IndexOf('-'));
            if ((stroka[0] == '-') || (stroka[1] == '-'))
            {
                stroka = stroka.Remove(stroka.IndexOf('-'), 1);
               if(stroka.Contains("-")) stroka = stroka.Remove(stroka.IndexOf('-'));
                stroka = stroka.Insert(0, "-");
            }
            string[] qwerty = new string[2];
                if (stroka.Contains("$"))
                {
                    qwerty[0] = "$";
                    stroka = stroka.Remove(stroka.IndexOf('$'), 1);
                    qwerty[1] = stroka;
                }
                if (stroka.Contains("eur"))
                {
                    qwerty[0] = "eur";
                    stroka = stroka.Remove(stroka.IndexOf('e'), 3);
                    qwerty[1] = stroka;
                }
            return qwerty;
        }
        public static string Value_Convert(string[] value, string[] kurs, string type)
        {
            string stroka = "";
            double temp, k;
            if ((type == "toDollar") && (value[0] == "eur"))
            {
                value[0] = "$";
                temp = Convert.ToDouble(value[1]);
                k = Convert.ToDouble(kurs[0]);
                temp = temp * k;
                value[1] = Convert.ToString(temp);
                stroka = string.Concat(value[0], value[1]);
            }
            else
            {
                if ((type == "toEuro") && (value[0] == "$"))
                {
                    value[0] = "eur";
                    temp = Convert.ToDouble(value[1]);
                    k = Convert.ToDouble(kurs[1]);
                    temp = temp * k;
                    value[1] = Convert.ToString(temp);
                    stroka = string.Concat(value[1], value[0]);
                }
                else Console.WriteLine("Несоответствие типов");
            }
            return stroka;
        }
        public static string[] Value_Plus(string[] value1, string[] value2)
        {
            if (value1[0] == value2[0])
            {
                double temp = Convert.ToDouble(value1[1]) + Convert.ToDouble(value2[1]);
                value1[1] = Convert.ToString(temp);
            }
            else
            {
                Console.WriteLine("Несоответствие типов");
                value1[0] = "";
                value1[1] = "";
            }
            return value1;
        }
        public static string[] Value_Minus(string[] value1, string[] value2)
        {
            if (value1[0] == value2[0])
            {
                double temp = Convert.ToDouble(value1[1]) - Convert.ToDouble(value2[1]);
                value1[1] = Convert.ToString(temp);
            }
            else
            {
                Console.WriteLine("Несоответствие типов");
                value1[0] = "";
                value1[1] = "";
            }
            return value1;
        }
    }
}
