using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Sudoku
{
    class Map
    {
        public int[,] sdk = new int[9, 9];
        public int[] x;
        public int[] y;
        public int[] m;


        public void Initial1()
        {
            x = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            y = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            m = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        public void Initial2()
        {
            sdk = new int[9, 9] { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 },{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },{ 0, 0, 0, 0, 0, 0, 0, 0, 0 }};
        }

        public void Output(ref string s, FileInfo finfo)
        {
  
            int i, j;
            using (FileStream fs = finfo.OpenWrite())
            {
                StreamWriter w = new StreamWriter(fs);
                w.BaseStream.Seek(0, SeekOrigin.End);

                for (i = 0; i < 9; i++)
                {
                    for (j = 0; j < 9; j++)
                    {
                        s += sdk[i, j].ToString() + " ";
                    }
                    w.WriteLine(s);
                    s = "";
                }
                w.WriteLine();
                w.Flush();
                w.Close();
            }
            s = "";

        }

    }
    class Program
    {
        const int first = 3;
        static int[] xp = new int[9] { 0, 1, 2, 0, 1, 2, 0, 1, 2 };
        static int[] yp = new int[9] { 0, 0, 0, 1, 1, 1, 2, 2, 2 };
        static int[] Xp = new int[9] { 0, 3, 6, 0, 3, 6, 0, 3, 6 };
        static int[] Yp = new int[9] { 0, 0, 0, 3, 3, 3, 6, 6, 6 };
        static int[] num = new int[9] { 3, 4, 5, 6, 7, 8, 9, 1, 2 };//填入九宫格数字的顺序(the order of the numbers filled in Nine Patch)

        static bool Fill(int s, int d, ref Map a,ref int count)//将数字随机填入九宫格(fill the Nine Patch with each number)
        {
            int i, j, m, n;
            Random ran = new Random(Guid.NewGuid().GetHashCode());
            for (j = 0; j < 6; j++)
            {
                i = ran.Next(0, 9);//随机生成一个九宫格中的位置(randomly generate the position to fill in Nine Patch)
                m = Yp[s] + yp[i];
                n = Xp[s] + xp[i];
                if (a.sdk[m, n] == 0 && a.x[n] == 0 && a.y[m] == 0)//判断是否可以填入数字(determine if the number can be filled in)
                {
                    count++;//每次填入时count+1(count + 1 every time you fill in)
                    a.sdk[m, n] = d;
                    a.y[m] = 1;
                    a.x[n] = 1;
                    a.m[s] = 1;
                    return true;
                }
            }
            return false;
        }

        static int Back(int s, int d, ref Map a)//当一个九宫格填不下去时，回到上一个九宫格(when you can't put the number in this Nine Patch,go back to the previous one)
        {
            int i, m, n;
            if (s == 0) return s;
            else s--;
            a.m[s] = 0;
            for (i = 0; i < 9; i++)
            {
                m = Yp[s] + yp[i];
                n = Xp[s] + xp[i];
                if (a.sdk[m, n] == d)
                {
                    a.sdk[m, n] = 0;
                    a.y[m] = 0;
                    a.x[n] = 0;
                    return s;
                }
            }
            return s;
        }

        static void Number(int d, int k, int s, ref Map a,ref int count)//将数字依次填入九宫格(fill Nine Patch with the number in order)
        {
            for (s = 0; s < 9; s++)
            {
                if (k == 0 && s == 0)
                {
                    a.sdk[0, 0] = d;
                    a.x[0] = 1;
                    a.y[0] = 1;
                    a.m[0] = 1;
                    continue; 
                }
                while (a.m[s] == 0)
                {
                    if (!Fill(s, d, ref a,ref count))
                    {
                        if (count > 500) return;//count大于500时跳出循环(drop the loop when count>500)
                        s=Back(s, d, ref a);
                        continue;
                    }
                }
            }
        }

        static void SuDoKu(int d, ref Map a,ref int count)//每个数字依次填入数独(each number fill in th Sudoku in order)
        {
            int k, s = 0; ;
            for (k = 0; k < 9; k++)
            {
                a.Initial1();
                Number(num[d + k], k, s, ref a,ref count);
                if (count >500) return;
            }
        }

        public static void Main(string[] args)
        {
            int n,count;
            string s="";
            n = Convert.ToInt32(args[1]);
            Map a = new Map();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sudoku.txt");
            if (!File.Exists(path)) File.Create(path);
            FileInfo finfo = new FileInfo(path);
            if (finfo.Exists)    finfo.Delete();
            for (; n > 0; )
            {
                count = 0;
                SuDoKu(0, ref a,ref count);
                if (count < 500)
                {
                    a.Output(ref s,finfo);
                    n--;
                }
                a.Initial1();
                a.Initial2();
            }
        }
    }
}
