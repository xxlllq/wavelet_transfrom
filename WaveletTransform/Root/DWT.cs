using System;
using System.Collections.Generic;
using System.Text;
using WaveletTransform.Util;

namespace WaveletTransform.Root
{
    /// <summary>
    /// 一维小波变换类
    /// </summary>
    public class DWT
    {
        /// <summary>
        /// 低通重构滤波器系数
        /// </summary>
        public List<double> Lo_R { set; get; }
        /// <summary>
        /// 高通重构滤波器系数
        /// </summary>
        public List<double> Hi_R { set; get; }
        /// <summary>
        /// 低通分解滤波器系数
        /// </summary>
        public List<double> Lo_D { set; get; }
        /// <summary>
        /// 高通分解滤波器系数
        /// </summary>
        public List<double> Hi_D { set; get; }
        /// <summary>
        /// 一维小波输入的声信号
        /// </summary>
        public List<double> Oelts { set; get; }
        /// <summary>
        /// 二维小波输入的声信号
        /// </summary>
        public List<List<double>> Telts { set; get; }
        public List<double> lprime { set; get; }


        /// <summary>
        /// 实例化一维DWT
        /// </summary>
        /// <param name="orginal">输入源信号</param>
        /// <param name="wname">haar、db1~db10</param>
        public DWT(List<double> orginal, string wname)
        {
            Oelts = orginal;
            lprime = GetLPrime(wname);
            GetFilters(this);
        }
        /// <summary>
        /// 实例化二维DWT
        /// </summary>
        /// <param name="orginal">输入源信号</param>
        /// <param name="wname">haar、db1~db10</param>
        public DWT(List<List<double>> orginal, string wname)
        {
            Telts = orginal;
            lprime = GetLPrime(wname);
            GetFilters(this);
        }

        public DWT(string wname)
        {
            lprime = GetLPrime(wname);
            GetFilters(this);
        }


        private List<double> GetLPrime(string wname)
        {
            List<double> lp = null;
            switch (wname)
            {
                case "haar":
                //Daubechies db
                case "dbl ": lp = new List<double> { 0.5, 0.5 }; break;
                case "db2": lp = new List<double> { 0.34151, 0.59151, 0.15849, -0.091506 }; break;
                case "db3": lp = new List<double> { 0.23523, 0.57056, 0.32518, -0.095467, -0.060416, 0.024909 }; break;
                case "db4": lp = new List<double> { 0.1629, 0.50547, 0.4461, -0.019788, -0.13225, 0.021808, 0.023252, -0.0074935 }; break;
                case "db5":
                    lp = new List<double>{
                0.11321, 0.42697, 0.51216, 0.097883, -0.17133, -0.022801, 0.054851, -0.0044134,
                -0.0088959, 0.0023587 }; break;
                case "db6":
                    lp = new List<double>{
                0.078871,0.34975,0.53113, 0.22292,
                -0.15999,-0.091759, 0.068944, 0.019462, -0.022332, 0.00039163,
                -0.003378,-0.00076177 }; break;
                case "db7":
                    lp = new List<double>{
                0.05505, 0.2804, 0.51557, 0.33219,
                -0.10176,-0.15842, 0.050423, 0.057002, -0.026891, -0.01172,
                0.0088749, 0.00030376, -0.001274, 0.00025011 }; break;
                case "db8":
                    lp = new List<double>{
                0.038478,0.22123,0.47774, 0.41391,
                -0.011193,-0.20083, 0.0003341, 0.091038, -0.012282,-0.031175,
                0.0098861, 0.0061844, -0.0034439, -0.000277, 0.00047761, -8.3069e-005 }; break;
                case "db9":
                    lp = new List<double>{
                0.026925,0.17242,0.42767, 0.46477, 0.094185,-0.20738, -0.068477, 0.10503, 0.021726, -0.047824,
                0.00017745, 0.015812, -0.0033398, -0.0030275, 0.0013065, 0.00016271, -0.00017816, 2.7823e-005 }; break;
                case "db10":
                    lp = new List<double> {
                0.018859,0.13306,0.37279,0.48681, 0.19882, -0.17667, -0.13855, 0.090064, 0.065801,-0.050483,
                -0.02083, 0.023485, 0.0025502, -0.0075895, 0.00098666,0.0014088,
                -0.00048497, -8.2355e-005, 6.6177e-005, -9.3792e-006 }; break;
                //Symlets
                case "sym2":
                    lp = new List<double> { 0.34151, 0.59151, 0.15849, -0.091506 }; break;
                case "sym3":
                    lp = new List<double> { 0.23523, 0.57056, 0.32518, -0.095467, -0.060416, 0.024909 }; break;
                case "sym4":
                    lp = new List<double> { 0.022785 - 0.0089124, -0.070159, 0.21062, 0.56833, 0.35187, -0.020955, -0.053574 }; break;
                case "sym5":
                    lp = new List<double> { 0.013816, -0.014921, -0.12398, 0.011739, 0.44829, 0.51153, 0.141, -0.027672, 0.020873, 0.019327 }; break;
                case "sym6":
                    lp = new List<double> { -0.0055159, 0.00125, 0.031625, -0.014892, -0.051362, 0.23895, 0.55695, 0.34723, -0.034162, -0.083432, 0.0024683, 0.010892 }; break;
                case "sym7":
                    lp = new List<double> { 0.0072607, 0.0028357, -0.076232, -0.099028, 0.20409, 0.54289, 0.37908, 0.012333, -0.035039, 0.048007, 0.021578, -0.0089352, -0.00074061, 0.0018963 }; break;
                case "sym8":
                    lp = new List<double> { 0.0013364 - 0.0002142, -0.010573, 0.0026932, 0.034745, -0.019247, -0.036731, 0.2577, 0.54955, 0.34037, -0.043327, -0.10132, 0.0053793, 0.022412, 0.00038335, -0.0023917 }; break;
                //Coiflets
                case "coif1":
                    lp = new List<double> { -0.05143, 0.23893, 0.60286, 0.27214, -0.05143, -0.01107 }; break;
                case "coif2":
                    lp = new List<double> { 0.011588, -0.02932, -0.04764, 0.27302, 0.57468, 0.29487, -0.054086, -0.042026, 0.016744, 0.0039679, -0.0012892, -0.00050951 }; break;
                case "coif3":
                    lp = new List<double> { -0.0026824, 0.0055031, 0.016584, -0.046508, -0.043221, 0.2865, 0.56129, 0.30298, -0.05077, -0.058196, 0.024434, 0.011229, -0.0063696, -0.0018205, 0.00079021, 0.00032967, -5.0193e-05 - 2.4466e-05 }; break;
                case "coif4":
                    lp = new List<double> { 0.00063096 - 0.0011522 - 0.0051945, 0.011362, 0.018867, -0.057464, -0.039653, 0.29367, 0.55313, 0.30716, -0.047113, -0.068038, 0.027814, 0.017736, -0.010756, -0.004001, 0.0026527, 0.00089559, -0.0004165, -0.00018383, 4.408e-05, 2.2083e-05, -2.3049e-06, -1.2622e-06 }; break;
                case "coif5":
                    lp = new List<double> { -0.00014996, 0.00025356, 0.0015402, -0.0029411, -0.0071638, 0.016552, 0.019918, -0.064997, -0.0368, 0.29809, 0.54751, 0.30971, -0.043866, -0.074652, 0.029196, 0.023111, -0.013974, -0.0064801, 0.004783, 0.0017207, -0.0011758, -0.00045123, 0.00021373, 9.9378e-05, -2.9232e-05, -1.5072e-05, 2.6408e-06, 1.4593e-06, -1.184e-07, -6.73e-08 }; break;
                //DMeyer
                case "dmey":
                    lp = new List<double> { -1.0675e-06, 9.0422e-07, 3.179e-07 - 1.4825e-06, 1.2185e-06, 4.9362e-07, -2.036e-06, 1.6851e-06, 6.9474e-07 - 2.9824e-06, 2.3713e-06, 1.1842e-06, -4.267e-06, 3.4207e-06, 1.6987e-06 - 6.7573e-06, 5.1029e-06, 3.4288e-06 - 1.0046e-05, 7.4274e-06, 4.3753e-06, -1.728e-05, 1.4217e-05, 1.0602e-05, -3.283e-05, 2.2869e-05, 2.6453e-05 - 7.2676e-05, 1.7297e-05, 0.00010586 - 5.3452e-05 - 9.8934e-05, -6.6124e-05, 0.00011398, 0.00060776 - 0.00040884, -0.0019107, 0.0015519, 0.0042748, -0.0045161, -0.0078097, 0.010784, 0.012306, -0.022694, -0.017198, 0.04502, 0.021652, -0.093831, -0.024783, 0.31402, 0.52591, 0.31402, -0.024783, -0.093831, 0.021652, 0.04502, -0.017198, -0.022694, 0.012306, 0.010784, -0.0078097, -0.0045161, 0.0042748, 0.0015519, -0.0019107 - 0.00040884, 0.00060776, 0.00011398 - 6.6124e-05 - 9.8934e-05 - 5.3452e-05, 0.00010586, 1.7297e-05 - 7.2676e-05, 2.6453e-05, 2.2869e-05, -3.283e-05, 1.0602e-05, 1.4217e-05, -1.728e-05, 4.3753e-06, 7.4274e-06, -1.0046e-05, 3.4288e-06, 5.1029e-06, -6.7573e-06, 1.6987e-06, 3.4207e-06, -4.267e-06, 1.1842e-06, 2.3713e-06, -2.9824e-06, 6.9474e-07, 1.6851e-06, -2.036e-06, 4.9362e-07, 1.2185e-06 - 1.4825e-06, 3.179e-07, 9.0422e-07, -1.0675e-06, 0 }; break;
                //Morlet
                case "morl":
                    lp = new List<double> { 0.018859, 0.13306, 0.37279, 0.48681, 0.19882, -0.17667, -0.13855, 0.090064, 0.065801, -0.050483, -0.02083, 0.023485, 0.0025502 - 0.0075895, 0.00098666, 0.0014088, -0.00048497 - 8.2355e-05, 6.6177e-05, -9.3792e-06 }; break;

                default: lp = new List<double> { 0.5, 0.5 }; break;
            }
            return lp;
        }

        /// <summary>
        /// 根据给定滤波器,产生两对孪生小波变换基对象
        /// </summary>
        /// <param name="w"></param>
        public DWT GetFilters(DWT w)
        {

            if (w != null && w.lprime != null)
            {
                double nw = 1 / Norm(w.lprime);
                w.Lo_R = nw * w;
                w.Hi_R = Qmf(w.Lo_R);
                w.Lo_D = Wrev(w.Lo_R);
                w.Hi_D = Wrev(w.Hi_R);
            }
            return w;
        }

        /// <summary>
        /// 数组倍数缩放
        /// </summary>
        /// <param name="d"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        static public List<double> operator *(double d, DWT s)
        {
            List<double> lps = s.lprime;
            for (int i = 0; i < lps.Count; i++) lps[i] *= d;
            return lps;
        }

        #region 辅助计算工具
        /// <summary>
        /// 反转
        /// </summary>
        /// <returns></returns>
        public List<double> Wrev(List<double> orginal)
        {
            int length = orginal.Count;
            List<double> res = new List<double>();
            for (int i = 0; i < length; i++)
                res.Add(orginal[length - 1 - i]);
            return res;
        }

        /// <summary>
        /// 积分镜像过滤
        /// </summary>
        /// <returns></returns>
        public List<double> Qmf(List<double> orginal)
        {
            //Quadrature mirror filter 
            return Altsign(Wrev(orginal));
        }

        /// <summary>
        /// 间隔变号
        /// </summary>
        /// <returns></returns>
        public List<double> Altsign(List<double> orginal)
        {
            List<double> res = CommonUtil.DeepCopyByBin(orginal);
            int sign = -1;
            for (int i = 1; i < res.Count; i += 2)
            {
                res[i] *= sign;
            }
            return res;
        }

        /// <summary>
        /// 模2
        /// </summary>
        /// <returns></returns>
        public double Norm(List<double> orginal)
        {
            double sum = 0.0;
            for (int i = 0; i < orginal.Count; i++)
            {
                sum += orginal[i] * orginal[i];
            }
            sum = Math.Sqrt(sum);
            return sum;
        }

        /// <summary>
        /// 合计
        /// </summary>
        /// <returns></returns>
        public double Sum(List<double> orginal)
        {
            double sum = 0.0;
            for (int i = 0; i < orginal.Count; i++)
            {
                sum += orginal[i];
            }
            return sum;
        }

        /// <summary>
        /// 边缘处理
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xsize"></param>
        /// <returns></returns>
        static private int Edge(int x, int xsize)
        {
            while ((x < 0) || (x >= xsize))
            {
                if (x < 0) x = -x;
                if (x >= xsize)
                    x = xsize * 2 - x - 2;
            }
            return x;
        }

        /// <summary>
        /// 返回间隔数据
        /// </summary>
        /// <returns></returns>
        public List<double> Space(int first, int last, List<double> orginal)
        {
            List<double> re = new List<double>();
            for (int i = first; i < last; i += 2)
            {
                re.Add(orginal[i]);
            }
            return re;
        }

        #endregion
    }
}
