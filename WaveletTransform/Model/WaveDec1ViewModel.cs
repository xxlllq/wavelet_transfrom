using System;
using System.Collections.Generic;
using System.Text;
using WaveletTransform.Root;

namespace WaveletTransform.Model
{
    /// <summary>
    /// 一维小波分解结果ViewModel
    /// </summary>
    public class WaveDec1ViewModel
    {
        /// <summary>
        /// 近似系数+细节系数
        /// </summary>
        public List<double> AppAndDetail { set; get; }

        /// <summary>
        /// 各层系数长度 
        /// </summary>
        public List<int> Coefficient { set; get; }

        /// <summary>
        /// 变换类
        /// </summary>
        public DWT Dwt { set; get; }
    }
}
