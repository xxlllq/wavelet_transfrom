using System;
using System.Collections.Generic;
using System.Text;

namespace WaveletTransform.Model
{
    /// <summary>
    /// 小波基函数实体类，待修改
    /// </summary>
    public class WavefunModel
    {
        public List<double> phi { set; get; }
        public List<double> psi { set; get; }
        public List<double> xval { set; get; }
        public double step { set; get; }
        public int type { set; get; }
        public List<double> out1 { set; get; }
        public List<double> out4 { set; get; }
        public List<double> out5 { set; get; }
    }
}
