using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveletTransform.Model;
using WaveletTransform.Root;

namespace WaveletTransform.Util
{
    /// <summary>
    /// 一维/二维小波变换
    /// </summary>
    public class DWTUtil
    {
        #region 一维离散小波变换
        /// <summary>
        /// 一维离散小波变换
        /// </summary>
        /// <param name="orginal">输入源信号</param>
        /// <param name="wname">haar、db1~db10</param>
        /// <returns>>近似系数+细节系数，各层系数长度</returns>
        public static List<List<double>> DWT1(List<double> orginal, string wname)
        {
            List<List<double>> result = new List<List<double>>();
            try
            {
                DWT dwt = new DWT(orginal, wname);
                if (dwt != null && dwt.Lo_D != null)
                {
                    int lfLength = dwt.Lo_D.Count - 1;
                    List<double> extend = TransformUtil.DWT1Extend("sym", orginal, lfLength);
                    if (extend != null && extend.Count > 0)
                    {
                        int last = dwt.Oelts.Count + lfLength;
                        result.Add(dwt.Space(1, last, TransformUtil.Convolve(extend, dwt.Lo_D, "valid")));
                        result.Add(dwt.Space(1, last, TransformUtil.Convolve(extend, dwt.Hi_D, "valid")));
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 一维离散小波变换
        /// </summary>
        /// <param name="orginal">输入源信号</param>
        /// <param name="dwt">变换类</param>
        /// <returns>当前近似系数(信号低频成分)+细节系数(信号高频成分)，各层系数长度</returns>
        public static List<List<double>> DWT1(List<double> orginal, DWT dwt)
        {
            List<List<double>> result = new List<List<double>>();
            if (orginal == null || dwt == null)
                return result;

            try
            {
                int lfLength = dwt.Lo_D.Count - 1;
                List<double> extend = TransformUtil.DWT1Extend("sym", orginal, lfLength);
                if (extend != null && extend.Count > 0)
                {
                    int last = orginal.Count + lfLength;
                    result.Add(dwt.Space(1, last, TransformUtil.Convolve(extend, dwt.Lo_D, "valid")));
                    result.Add(dwt.Space(1, last, TransformUtil.Convolve(extend, dwt.Hi_D, "valid")));
                }
            }
            catch (Exception ex) { }
            return result;
        }
        #endregion 

        #region 一维小波逆变换
        /// <summary>
        /// 一维小波逆变换
        /// </summary>
        /// <param name="a"> </param>
        /// <param name="b"> </param>
        /// <param name="dwt">变换类</param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static List<double> IDWT1(List<double> a, List<double> d, DWT dwt, int p)
        {
            List<double> upo = Upsconv1(a, dwt.Lo_R, p);
            List<double> upt = Upsconv1(d, dwt.Hi_R, p);
            if (upo != null && upt != null && upo.Any() && upt.Any() && upo.Count == upt.Count)
            {
                for (int i = 0; i < upo.Count; i++)
                    upo[i] = upo[i] + upt[i];

            }
            return upo;
        }

        /// <summary>
        /// 一维小波逆变换
        /// </summary>
        /// <param name="a"> </param>
        /// <param name="b"> </param>
        /// <param name="wname">haar、db1~db10</param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static List<double> IDWT1(List<double> a, List<double> b, string wname, int p)
        {
            return IDWT1(a, b, new DWT(wname), p);
        }

        #region 一维小波逆变换辅助函数

        public static List<double> Upsconv1(List<double> x, List<double> f, int p)
        {
            List<double> data = Wconv1(Dyadup(x, 0), f);
            if (data != null && data.Any())
                data = Wkeep1(data, p, "c");
            return data;
        }

        public static List<double> Wconv1(List<double> x, List<double> f)
        {
            return TransformUtil.Convolve(x, f, "full");
        }

        public static List<double> Wkeep1(List<double> x, int len, string type = null)
        {
            int sx = x.Count;
            if (!(len >= 0 && len < sx))
                return x;

            double d = (sx - len) / (double)2;
            int first = 0, last = 0;
            if ("c".Equals(type))
            {
                first = Convert.ToInt32(Math.Floor(d));
                last = Convert.ToInt32(sx - Math.Ceiling(d));
                if (last >= first)
                    x = x.GetRange(first, last - first);
            }
            return x;
        }

        static List<double> Dyadup(List<double> y, int p, int q = -1)
        {
            int evenLEN = 0, addLEN = 0;
            if (q >= 0)
                evenLEN = 1;

            int rem2 = p % 2;
            if (evenLEN != 0)
                addLEN = 0;
            else
                addLEN = 2 * rem2 - 1;

            int len = 2 * y.Count + addLEN;
            double[] result = new double[len];
            for (int i = rem2; i < len; i += 2)
                result[i] = y[i / 2];

            return result.ToList();
        }
        #endregion
        #endregion

        #region 一维连续小波变换
        /// <summary>
        /// 连续小波变换
        /// </summary>
        /// <param name="sig">输入信号</param>
        /// <param name="scals">尺度</param>
        /// <param name="wname">小波名称</param>
        /// <returns></returns>
        public static CWTModel CWT(List<double> sig, int totalscal, string wname, double dt)
        {
            CWTModel cwtModel = new CWTModel();
            try
            {
                if (sig != null && sig.Any() && totalscal > 0)
                {
                    int lenSIG = sig.Count, stepSIG = 1;
                    WaveletsInfoModel fc = TransformUtil.CentFrq(wname);//获取中心频率
                    if (fc != null)
                    {
                        double cff = 2 * fc.centerfc * totalscal;
                        List<double> scals = new List<double>();
                        for (int i = 1; i <= totalscal; i++)
                            scals.Add(cff / i);

                        cwtModel.scal2frq = TransformUtil.Scal2Frq(scals, fc.centerfc, dt);//尺度转频率
                        List<int> xSIG = Enumerable.Range(1, lenSIG).ToList();

                        WavefunModel wave = TransformUtil.Intwave(fc, wname, 10);
                        if (wave != null)
                        {
                            List<double> valWAV = wave.out1, xWAV = wave.xval;
                            if (xWAV != null && xWAV.Any())
                            {
                                double xwav = xWAV[0];
                                xWAV = xWAV.Select(m => m - xwav).ToList();
                                double xMaxWAV = xWAV.Last();

                                int nbSCALES = scals.Count;
                                List<List<double>> coefs = new List<List<double>>();
                                for (int k = 0; k < nbSCALES; k++)
                                {
                                    double a = scals[k];
                                    double aSIG = a / stepSIG;
                                    double ax = aSIG * xMaxWAV;

                                    List<double> re = new List<double>();
                                    for (double i = ax; i >= 0; i--)
                                    {
                                        re.Add(valWAV[(int)Math.Floor(i / (aSIG * wave.step))]);
                                    }
                                    if (re.Count == 1)
                                        re = new List<double> { 1, 1 };

                                    List<double> wn = Wconv1(sig, re);
                                    if (wn != null && wn.Any())
                                    {
                                        int count = wn.Count;
                                        for (int i = 0; i < count - 1; i++)
                                        {
                                            wn[i] = wn[i + 1] - wn[i];
                                        }
                                        wn.RemoveAt(count - 1);
                                        coefs.Add(Wkeep1(wn, lenSIG, "c").Select(m => m * (-Math.Sqrt(a))).ToList());
                                    }
                                }
                                cwtModel.cwt = coefs;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cwtModel;
        }
        #endregion

        #region 二维小波变换
        /// <summary>
        /// 二维小波变换
        /// </summary>
        /// <param name="orginal">输入源信号</param>
        /// <param name="wname">haar、db1~db10</param>
        /// <returns>cA(近似分量、Dictionary位置1)，cH(水平细节分量、Dictionary位置2),cV(垂直细节分量、Dictionary位置3),cD(对角细节分量、Dictionary位置4)</returns>
        public static Dictionary<int, List<List<double>>> DWT2(List<List<double>> orginal, string wname)
        {
            Dictionary<int, List<List<double>>> result = new Dictionary<int, List<List<double>>>();
            try
            {
                DWT dwt = new DWT(orginal, wname);
                if (dwt != null && dwt.Lo_D != null && dwt.Hi_D != null)
                {
                    int lodl = dwt.Lo_D.Count - 1, hidl = dwt.Hi_D.Count - 1, orginalRow = orginal.Count, orginalCol = orginal[0].Count;
                    List<List<double>> y = TransformUtil.DWT2Extend("addcol", "sym", orginal, lodl);
                    List<List<double>> convresult = TransformUtil.Convolve(y, dwt.Lo_D, "valid");


                    //近似分量
                    result.Add(0, TransformUtil.ConvDown(convresult, dwt.Lo_D, lodl, orginalRow, orginalCol));
                    //水平细节分量
                    result.Add(1, TransformUtil.ConvDown(convresult, dwt.Hi_D, hidl, orginalRow, orginalCol));
                    convresult = TransformUtil.Convolve(y, dwt.Hi_D, "valid");
                    //垂直细节分量
                    result.Add(2, TransformUtil.ConvDown(convresult, dwt.Lo_D, lodl, orginalRow, orginalCol));
                    //对角细节分量
                    result.Add(4, TransformUtil.ConvDown(convresult, dwt.Hi_D, hidl, orginalRow, orginalCol));
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 二维小波变换
        /// </summary>
        /// <param name="orginal">输入源信号</param>
        /// <param name="dwt">变换类</param>
        /// <returns></returns>
        public static Dictionary<int, List<List<double>>> DWT2(List<List<double>> orginal, DWT dwt)
        {
            Dictionary<int, List<List<double>>> result = new Dictionary<int, List<List<double>>>();
            try
            {
                int lodl = dwt.Lo_D.Count - 1, hidl = dwt.Hi_D.Count - 1, orginalRow = orginal.Count, orginalCol = orginal[0].Count;
                List<List<double>> y = TransformUtil.DWT2Extend("addcol", "sym", orginal, lodl);
                List<List<double>> convresult = TransformUtil.Convolve(y, dwt.Lo_D, "valid");

                //近似分量
                result.Add(0, TransformUtil.ConvDown(convresult, dwt.Lo_D, lodl, orginalRow, orginalCol));
                //水平细节分量
                result.Add(1, TransformUtil.ConvDown(convresult, dwt.Hi_D, hidl, orginalRow, orginalCol));
                convresult = TransformUtil.Convolve(y, dwt.Hi_D, "valid");
                //垂直细节分量
                result.Add(2, TransformUtil.ConvDown(convresult, dwt.Lo_D, lodl, orginalRow, orginalCol));
                result.Add(3, TransformUtil.ConvDown(convresult, dwt.Hi_D, hidl, orginalRow, orginalCol));
            }
            catch (Exception ex) { }
            return result;
        }
        #endregion

        #region 辅助工具类
        #region 提取近似系数
        /// <summary>
        /// 提取近似系数
        /// </summary>
        /// <param name="x">一维小波分解后的结果</param>
        /// <param name="l">层级系数</param>
        /// <param name="dwt">变换类</param>
        /// <param name="i">0 <= i <= length(l)-2</param>
        public static List<double> Appcoef(List<double> x, List<int> l, DWT dwt, int i = -1)
        {
            List<double> result = new List<double>();
            if (x != null && l != null && dwt != null)
            {
                int rmax = l.Count;
                int nmax = rmax - 2, n = nmax;
                if (i >= 0 && i <= nmax)
                    n = i;
                result = x.GetRange(0, l[0]);

                int imax = rmax + 1;
                for (int j = nmax; j >= n + 1; j--)
                {
                    List<double> d = Detcoef(x, l, j);
                    result = IDWT1(result, d, dwt, l[imax - j]);
                }

            }
            return result;
        }

        /// <summary>
        /// 提取近似系数
        /// </summary>
        /// <param name="x">一维小波分解后的结果</param>
        /// <param name="l">层级系数</param>
        /// <param name="wname">haar、db1~db10</param>
        public static List<double> Appcoef(List<double> x, List<int> l, string wname)
        {
            return Appcoef(x, l, new DWT(wname));
        }
        #endregion

        #region 提取细节系数
        /// <summary>
        /// 提取细节系数
        /// </summary>
        /// <param name="c">一维小波分解后的结果</param>
        /// <param name="l">层级系数</param>
        /// <param name="p">层级数</param>
        /// <returns></returns>
        public static List<double> Detcoef(List<double> c, List<int> l, int p)
        {
            List<int> first = new List<int>(), longs = new List<int>(), last = new List<int>();
            int n = l.Count;
            for (int i = n - 2; i >= 1; i--)
                first.Add(l.GetRange(0, i).Sum());

            for (int i = n - 1; i >= 2; i--)
                longs.Add(l[i - 1]);

            for (int i = 0; i < longs.Count; i++)
                last.Add(longs[i] + first[i]);

            p -= 1;
            if (p >= 0 && p < first.Count)
            {
                int findex = first[p], lindex = last[p];
                int count = lindex - findex;
                c = c.GetRange(findex, count >= 0 ? count : 0);
            }
            return c;
        }
        #endregion

        #region 近似系数重构/细节系数重构
        /// <summary>
        /// 近似系数重构/细节系数重构
        /// </summary>
        /// <param name="type">a：approximate近似；d：detail细节</param>
        /// <param name="model"></param>
        /// <param name="lev"></param>
        /// <returns></returns>
        public static List<double> Wrcoef(string type, WaveDec1ViewModel model, int lev)
        {
            List<double> result = new List<double>(), f1 = null;
            if (model != null)
            {
                if ("a".Equals(type))
                {
                    result = Appcoef(model.AppAndDetail, model.Coefficient, model.Dwt, lev);
                    f1 = model.Dwt.Lo_R;
                }
                else if ("d".Equals(type))
                {
                    result = Detcoef(model.AppAndDetail, model.Coefficient, lev);
                    f1 = model.Dwt.Hi_R;
                }

                int rmax = model.Coefficient.Count, namx = rmax - 2;
                int imin = rmax - lev;
                result = Upsconv1(result, f1, model.Coefficient[imin]);
                for (int i = 2; i <= lev; i++)
                    result = Upsconv1(result, model.Dwt.Lo_R, model.Coefficient[imin + i - 1]);
            }

            return result;
        }

        /// <summary>
        /// 一维小波系数直接构造
        /// </summary>
        /// <param name="type">"a"(近似系数重构)或"d(细节系数重构)"</param>
        /// <param name="model"></param>
        /// <param name="iter">迭代次数</param>
        /// <returns></returns>
        public static List<double> Upcoef(string type, DWT dwt, int iter)
        {
            List<double> result = new List<double>(), f1 = null;
            if (dwt != null)
            {
                List<double> lor = dwt.Lo_R, hir = dwt.Hi_R;
                if ("a".Equals(type))
                    f1 = lor;
                else if ("d".Equals(type))
                    f1 = hir;

                int lf = lor.Count;
                int ly = lf;

                result.Add(1);
                result = Upsconv1(result, f1, ly);
                for (int k = 2; k <= iter; k++)
                {
                    ly = 2 * result.Count + lf - 2;
                    result = Upsconv1(result, lor, ly);
                }
            }
            return result;
        }
        #endregion

        #endregion
    }
}
