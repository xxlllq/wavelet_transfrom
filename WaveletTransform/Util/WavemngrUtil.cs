using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveletTransform.Model;
using WaveletTransform.Root;

namespace WaveletTransform.Util
{

    /// <summary>
    /// 小波管理器
    /// </summary>
    public class WavemngrUtil
    {
        /// <summary>
        /// 操作
        /// </summary>
        /// <param name="option">操作代号</param>
        /// <param name="wname">小波名称</param>
        public static WaveletsInfoModel Command(string option, string wname)
        {
            WaveletsInfoModel varargout = new WaveletsInfoModel();
            if (string.IsNullOrEmpty(option) || string.IsNullOrEmpty(wname))
                return varargout;
            try
            {
                List<WaveletsInfoModel> infos = varargout.GetWaveletsInfos();
                int okwave = 0, addnum = 0;
                if (infos != null && infos.Any())
                {
                    switch (option)
                    {
                        case "type":
                            goto case "indw";
                        case "indw":
                            int nbfam = infos.Count, lwna = wname.Length, globalifam = 0;//info长度，wname长度
                            for (int ifam = 0; ifam < nbfam; ifam++)
                            {
                                globalifam = ifam;
                                string fam = infos[ifam].familyShortName;//familyShortName
                                if (fam != null)
                                {
                                    int len = fam.Length;
                                    if (lwna >= len)
                                    {
                                        if (fam == wname.Substring(0, len))
                                        {
                                            string[] tabNums = infos[ifam].tabNums;
                                            string numstr = "";
                                            if (tabNums != null && tabNums.Any())
                                            {
                                                for (int inum = 0; inum < tabNums.Count(); inum++)
                                                {
                                                    numstr = tabNums[inum];
                                                    if ("no".Equals(numstr))
                                                        numstr = "";
                                                    if (wname.Equals(fam + numstr))
                                                    {
                                                        okwave = 1;
                                                        break;
                                                    }
                                                }

                                                //% test for ** number
                                                if (okwave == 0 && "**".Equals(numstr) && (lwna > len))
                                                {
                                                    string typNums = infos[ifam].typNums;
                                                    numstr = wname.Substring(len + 1, lwna);
                                                    switch (typNums)
                                                    {
                                                        case "integer":
                                                            double numi = Convert.ToDouble(numstr);
                                                            double numex = numi < 0 ? Math.Ceiling(numi) : Math.Floor(numi);
                                                            if ((numi == numex) && (0 < numi))
                                                                okwave = 1; addnum = 1;
                                                            break;
                                                        case "real":
                                                            int num = Convert.ToInt32(numstr);
                                                            if (num >= 0)
                                                                okwave = 1; addnum = 1;
                                                            break;
                                                        case "string":
                                                            okwave = 1; addnum = 1;
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    if (okwave == 1) break;
                                }
                            }

                            if (okwave == 1)
                                varargout = infos[globalifam];
                            //varargout.Add(globalinum);
                            break;
                        case "fields":
                            goto case "indw";
                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return varargout;
        }

        /// <summary>
        /// 小波基函数
        /// </summary>
        /// <param name="wname">小波名称</param>
        /// <param name="infoModel">小波信息</param>
        /// <param name="iter">迭代次数</param>
        public static WavefunModel Wavefun(string wname, WaveletsInfoModel infoModel, int iter)
        {
            WavefunModel data = new WavefunModel();
            if (!string.IsNullOrEmpty(wname))
            {
                string debut = wname.Substring(0, 2);
                double coef = Math.Pow(Math.Sqrt(2), iter), pas = (double)1 / Math.Pow(2, iter);
                switch (infoModel.type)
                {
                    case 1:
                        DWT dwt = new DWT(wname);
                        int logn = dwt.Lo_R.Count;
                        int nbpts = Convert.ToInt32((logn - 1) / pas + 1); 

                        List<double> psi = DWTUtil.Upcoef("d", dwt, iter);
                        if (psi != null && psi.Any())
                            psi = psi.Select(m => m * coef).ToList();
                        List<int> result = GetNBpts(nbpts, iter, logn);//nbpts、nb、dn

                        int nb = result[1], dn = result[2] + 1;
                        nbpts = result[0]; 
                        DWTUtil.Wkeep1(psi, nb, "c"); 
                        psi.Insert(0, 0);
                        for (int i = 1; i <= dn; i++)
                        { 
                            psi.Add(0);
                        }

                        //sign depends on wavelet
                        if ("co".Equals(debut) || "sy".Equals(debut) || "dm".Equals(debut))
                            psi = psi.Select(m => -m).ToList();

                        //data.phi = phi;
                        data.psi = psi;
                        data.type = infoModel.type;
                        data.xval = Linspace(0, (nbpts - 1) * pas, nbpts);
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        data = Morlet(infoModel.bounds[0], infoModel.bounds[1], (int)Math.Pow(2, iter));
                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
            }
            return data;
        }


        /// <summary>
        /// 产生d1和d2之间等间隔的n个数
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="n"></param>
        static List<double> Linspace(double d1, double d2, int n = 100)
        {
            int n1 = n - 1;
            double c = (d2 - d1) / n1;
            List<double> result = new List<double>();
            for (int i = 0; i < n1; i++)
                result.Add(d1 + i * c);
            result.Add(d2);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbpts"></param>
        /// <param name="iter"></param>
        /// <param name="logn"></param>
        /// <returns>nbpts+nb+dn</returns>
        static List<int> GetNBpts(int nbpts, int iter, int logn)
        {
            int lplus = logn - 2, nb = 1;
            for (int i = 1; i <= iter; i++)
                nb = 2 * nb + lplus;
            int dn = nbpts - nb - 2;
            if (dn < 0) { nbpts = nbpts - dn; dn = 0; }
            return new List<int> { nbpts, nb, dn };
        }

        static WavefunModel Morlet(int lb, int ub, int n)
        {
            List<double> linspace = Linspace(lb, ub, n), psi = new List<double>();
            for (int i = 0; i < linspace.Count; i++)
            {
                double item = linspace[i];
                psi.Add(Math.Exp(-(item * item) / 2) * Math.Cos(5 * item));
            }

            return new WavefunModel
            {
                psi = psi,
                xval = linspace
            };
        }
    }
}
