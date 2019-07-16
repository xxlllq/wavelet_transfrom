using System;
using System.Collections.Generic;
using System.Text;

namespace WaveletTransform.Model
{
    /// <summary>
    /// 一维连续变换Model类
    /// </summary>
    public class CWTModel
    {
        public List<double> times { set; get; }
        public List<double> scal2frq { set; get; }
        public List<List<double>> cwt { set; get; }
    }
}
