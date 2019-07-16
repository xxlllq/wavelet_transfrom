using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveletTransform.Model;
using WaveletTransform.Root;

namespace WaveletTransform.Util
{
    /// <summary>
    /// 一维/二维离散小波分解
    /// </summary>
    public class WaveDecUtil
    {
        #region 一维离散小波分解
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orginal">输入源信号</param>
        /// <param name="lev">分解层级</param>
        /// <param name="wname">haar、db1~db10</param>
        /// <returns>各层近似系数+细节系数，各层系数</returns>
        public static WaveDec1ViewModel WaveDec1(List<double> orginal, int lev = 1, string wname = "haar")
        {
            WaveDec1ViewModel result = new WaveDec1ViewModel();
            try
            {
                DWT dwt = new DWT(orginal, wname);//获取到相关的分解系数
                List<double> co = new List<double>();
                int[] li = new int[lev + 2];
                li[li.Length - 1] = orginal.Count;
                for (int i = 0; i < lev; i++)
                {
                    List<List<double>> data = DWTUtil.DWT1(orginal, dwt);
                    if (data != null && data.Count == 2)
                    {
                        orginal = data[0];
                        List<double> re = data[1];
                        if (re != null)
                        {
                            co = re.Concat(co).ToList();
                            li[lev - i] = re.Count;
                        }
                    }
                }

                co = orginal.Concat(co).ToList();
                li[0] = orginal.Count;
                result.AppAndDetail = co;
                result.Coefficient = li.ToList();
                result.Dwt = dwt;
            }
            catch (Exception ex)
            {

            }
            return result;

        }
        #endregion

        #region 二维离散小波分解
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orginal">输入源信号</param>
        /// <param name="lev">分解层级</param>
        /// <param name="wname">haar、db1~db10</param>
        /// <returns></returns>
        public static List<object> WaveDec2(List<List<double>> orginal, int lev = 1, string wname = "haar")
        {
            List<object> result = new List<object>();
            try
            {
                if (orginal != null && orginal.Any())
                {
                    DWT dwt = new DWT(orginal, wname);//获取到相关的分解系数
                    if (dwt != null && dwt.Lo_D != null && dwt.Hi_D != null && dwt.Lo_D.Count == dwt.Hi_D.Count)
                    {
                        List<double> cl = new List<double>();
                        List<List<double>> s = new List<List<double>>();
                        int r = lev + 2, c = orginal.Count;
                        for (int i = 0; i < r - 1; i++)
                        {
                            List<double> item = new List<double>();
                            for (int j = 0; j < 2; j++) item.Add(0);
                            s.Add(item);
                        }
                        s.Add(new List<double> { c, orginal[0].Count });

                        for (int i = 1; i <= lev; i++)
                        {
                            Dictionary<int, List<List<double>>> data = DWTUtil.DWT2(orginal, dwt);
                            if (data != null && data.Count == 4)
                            {
                                //近似分量
                                orginal = data[0];
                                //水平细节分量
                                List<List<double>> ch = data[1];
                                //垂直细节分量
                                List<List<double>> cv = data[2];
                                //对角细节分量
                                List<List<double>> cd = data[3];

                                //可优化，没有操作ca，看DWT2是否需要进行ca的值获取
                                cl = ConvertToLA(ch).Concat(ConvertToLA(cv)).Concat(ConvertToLA(cd)).Concat(cl).ToList();

                                s[r - i - 1] = new List<double> { orginal.Count, orginal[0].Count };
                            }
                        }
                        s[0] = new List<double> { orginal.Count, orginal[0].Count };

                        result.Add(ConvertToLA(orginal).Concat(cl).ToList());
                        result.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                var sd = ex;
            }
            return result;

        }

        /// <summary>
        /// 二维集合合并成列向量
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        static List<double> ConvertToLA(List<List<double>> or)
        {
            List<double> result = new List<double>();
            if (or != null && or.Any())
            {
                int row = or.Count, col = or[0].Count;
                for (int i = 0; i < col; i++)
                    for (int j = 0; j < row; j++)
                        result.Add(or[j][i]);
            }

            return result;
        }
        #endregion
    }
}
