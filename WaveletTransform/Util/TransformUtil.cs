using FFTW.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using WaveletTransform.Model;

namespace WaveletTransform.Util
{
    public class TransformUtil
    {
        #region 离散小波变换
        /// <summary>
        /// 一维小波变换中的延拓方法
        /// </summary>
        /// <param name="mode">延拓模型：默认为sym，对称模型</param>
        /// <param name="xd">输入源信号</param>
        /// <param name="lf"></param>
        /// <returns></returns>
        public static List<double> DWT1Extend(string mode, List<double> xd, int lf)
        {
            if (xd == null || string.IsNullOrEmpty(mode))
                return null;

            int[] indexs = GetSymIndices(xd.Count, lf, true);
            List<double> result = new List<double>();
            for (int i = 0; i < indexs.Length; i++)//优化，看能不能直接在原来的数组基础上操作
                result.Add(xd.ElementAtOrDefault(indexs[i] - 1));
            return result;
        }

        /// <summary>
        /// 二维小波变换中的延拓方法
        /// </summary>
        /// <param name="type">2d：二维；addrow：添加行；addcol：添加列。</param>
        /// <param name="mode">延拓模型：默认为sym，对称模型</param>
        /// <param name="xd">输入源信号</param>
        /// <param name="lf"></param>
        /// <returns></returns>
        public static List<List<double>> DWT2Extend(string type, string mode, List<List<double>> xd, int lf, string location = null)
        {
            if (xd == null || string.IsNullOrEmpty(mode))
                return null;

            List<List<double>> result = new List<List<double>>();
            try
            {
                if ("2d".Equals(type) && !string.IsNullOrEmpty(location))//二维小波变换入口
                {
                    int row = xd.Count, col = xd[0].Count;
                    if (location.Equals("nb"))
                    {//addcol
                     //获取数组的列数
                        int[] indexs = GetSymIndices(col, lf, true);
                        for (int i = 0; i < row; i++)
                        {
                            List<double> item = new List<double>();
                            for (int j = 0; j < indexs.Length; j++)//优化，看能不能直接在原来的数组基础上操作
                                item.Add(xd[i][indexs[j] - 1]);

                            result.Add(item);
                        }
                    }
                    else if (location.Equals("bn"))
                    {//addrow
                        int[] indexs = GetSymIndices(row, lf, true);
                        for (int i = 0; i < col; i++)
                        {
                            List<double> item = new List<double>();
                            for (int j = 0; j < indexs.Length; j++)
                                item.Add(xd[indexs[j] - 1][i]);
                            result.Add(item);
                        }
                    }
                    return result;
                }
                else if ("addrow".Equals(type))
                {
                    location = "bn";
                    return DWT2Extend("2d", "sym", xd, lf, location);
                }
                else if ("addcol".Equals(type))
                {
                    location = "nb";
                    return DWT2Extend("2d", "sym", xd, lf, location);
                }
            }
            catch (Exception ex)
            {
                var ds = ex;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="lf"></param>
        /// <param name="needConcat"></param>
        /// <returns></returns>
        static int[] GetSymIndices(int length, int lf, bool needConcat = false)
        {
            List<int> or = Enumerable.Range(1, length).ToList();
            List<int> re = new List<int>();
            //I = [lf: -1:1, 1:lx, lx: -1:lx - lf + 1];
            if (needConcat == true)
                for (int i = lf; i > 0; i--) re.Add(i);

            for (int i = length; i > length - lf; i--) or.Add(i);

            if (needConcat == true)
                or = re.Concat(or).ToList();

            if (length < lf)
            {
                or = or.Select(m => m < 1 ? 1 - m : m).ToList();//判断x的每一个元素< 1
                while (or.Any(m => m > length))//待优化
                {
                    or = or.Select(m => m > length ? 2 * length - m + 1 : m).ToList();//判断x的每一个元//判断x的每一个元素> 
                    if (or != null && or.Any())
                    {
                        or = or.Select(m => m < 1 ? 1 - m : m).ToList();
                    }
                }
            }

            return or.ToArray();
        }

        /// <summary>
        /// 一维小波卷积计算
        /// </summary>
        /// <param name="u">源数据</param>
        /// <param name="v">卷积核</param>
        /// <param name="shape">shape：full，same，valid</param>
        /// <returns></returns>
        public static List<double> Convolve(List<double> u, List<double> v, string shape)
        {
            int m = u.Count;
            int n = v.Count;
            int k = m + n - 1, start = 0, nm = n - 1, mm = m - 1;

            double[] w = new double[k];
            for (int i = 0; i < k; i++)
            {
                int kmin = (i >= nm) ? i - nm : 0;
                int kmax = (i < mm) ? i : mm;
                for (int j = kmin; j <= kmax; j++)
                {
                    w[i] += u[j] * v[i - j];
                }
            }

            List<double> re = w.ToList();
            if (shape == "valid")
            {
                //k = m - n + 1;
                //start = ((m + n - 1) - (m - n + 1)) / 2;
                start = n - 1;
                re.RemoveRange(k - start, start);
                re.RemoveRange(0, start);
            }
            else if (shape == "same")
            {
                //k = m;
                ////start = ((m + n - 1) - (m)) / 2;
                start = (n - 1) / 2;
                re.RemoveRange(0, start);
                re.RemoveRange(k - start, start);
            }
            return re;
        }

        /// <summary>
        /// 二维小波卷积计算
        /// </summary>
        /// <param name="u">源数据</param>
        /// <param name="v">卷积核</param>
        /// <param name="shape">shape：full，same，valid</param>
        public static List<List<double>> Convolve(List<List<double>> u, List<double> v, string shape)
        {
            List<List<double>> result = new List<List<double>>();
            if (u != null)
                for (int i = 0; i < u.Count; i++)
                    result.Add(Convolve(u[i], v, shape));
            return result;
        }

        /// <summary>
        /// 向量卷积
        /// </summary>
        /// <param name="x"></param>
        /// <param name="f"></param>
        /// <param name="l"></param>
        public static List<List<double>> ConvDown(List<List<double>> x, List<double> f, int l, int rownum, int colnum)
        {
            List<List<double>> result = new List<List<double>>();
            try
            {
                if (x != null && x.Any())
                {
                    int last1 = l + rownum, last2 = l + colnum;
                    for (int i = 0; i < rownum; i++)
                    {
                        List<double> item = x[i]; List<double> re = new List<double>();
                        for (int j = 1; j < last2; j += 2)
                            re.Add(item[j]);
                        result.Add(re);
                    }

                    List<List<double>> extrend = DWT2Extend("addrow", "sym", result, l);
                    if (extrend != null && extrend.Any())
                    {
                        extrend = Convolve(extrend, f, "valid");
                        //转置后间隔获取数据
                        int row = extrend.Count;
                        result.Clear();
                        for (int i = 1; i < last1; i += 2)
                        {
                            List<double> item = new List<double>();
                            for (int j = 0; j < row; j++)
                                item.Add(extrend[j][i]);
                            result.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var sd = ex;
            }
            return result;
        }
        #endregion

        #region 连续小波变换
        /// <summary>
        /// 计算以wname命名的母小波的中心频率
        /// </summary>
        /// <param name="wname">小波名称</param>
        /// <param name="iter">迭代次数</param>
        /// <returns></returns>
        public static WaveletsInfoModel CentFrq(string wname, int iter = 8)
        {
            WaveletsInfoModel infoModel = WavemngrUtil.Command("type", wname);
            if (infoModel != null)
            {
                try
                {
                    WavefunModel wf = WavemngrUtil.Wavefun(wname, infoModel, iter);
                    if (wf != null)
                    {
                        List<double> xval = wf.xval, psi = wf.psi;
                        double T = xval.Max() - xval.Min(), meanpsi = psi.Average();
                        int n = psi.Count;
                        var sdsd = psi.Max();
                        psi = psi.Select(m => m - meanpsi).ToList();

                        ///-----------------------------要修改
                        Complex[] sdsw = new Complex[n];
                        Complex[] sdswo = new Complex[n];
                        for (int j = 0; j < n; j++)
                        {
                            sdsw[j] = new Complex(psi[j], 0);
                        }
                        var input = new PinnedArray<Complex>(sdsw);
                        var output = new PinnedArray<Complex>(sdswo);

                        List<double> sp = new List<double>();
                        DFT.FFT(input, output);

                        int len = output.Buffer.Length;
                        for (int i = 0; i < len; i++)
                        {
                            Complex val = (Complex)output.Buffer.GetValue(i);
                            sp.Add(Math.Sqrt(val.Real * val.Real + val.Imaginary * val.Imaginary));
                        }

                        double vmax = sp.Max();//最大值
                        int indmax = sp.IndexOf(vmax);//下标
                        if (indmax > (double)n / 2)
                            indmax = n - indmax;

                        infoModel.centerfc = (double)indmax / T;
                    }
                }
                catch (Exception ex)
                {
                }
                //switch (wtype)
                //{
                //    case 1:
                //        break;
                //    case 2:
                //        break;
                //    case 3:
                //        break;
                //    case 4:
                //        break;
                //    case 5:
                //        break;
                //    default:
                //        break;
                //}
            }
            return infoModel;
        }

        /// <summary>
        /// 尺度转换为实际频率
        /// </summary>
        /// <param name="scal">尺度</param>
        /// <param name="centfrq">中心頻率</param>
        /// <param name="delta">采样周期</param>
        /// <returns></returns>
        public static List<double> Scal2Frq(List<double> scal, double centfrq, double delta = 1)
        {
            if (scal != null && scal.Any())
                scal = scal.Select(m => centfrq / (m * delta)).ToList();

            return scal;
        }

        /// <summary>
        /// 积分小波函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="wname"></param>
        /// <param name="precis"></param>
        public static WavefunModel Intwave(WaveletsInfoModel infoModel, string wname, int precis)
        {
            WavefunModel wf = null;
            try
            {
                wf = WavemngrUtil.Wavefun(wname, infoModel, precis);
                if (wf != null)
                {
                    int type = infoModel.type;
                    List<double> xval = wf.xval, psi = wf.psi;
                    if (xval != null && xval.Count > 1)
                    {
                        double step = wf.step = xval[1] - xval[0], temp = 0;
                        if (type == 1 || type == 3 || type == 4 || type == 5)
                        {
                            List<double> out1 = new List<double>();
                            for (int i = 0; i < psi.Count; i++)
                            {
                                temp += psi[i];
                                out1.Add(temp * step);
                            }

                            wf.out1 = out1;
                            return wf;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var sd = ex;
            }
            return null;
        }
    }
    #endregion
}
