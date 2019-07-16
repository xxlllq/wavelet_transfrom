using System;
using System.Collections.Generic;
using System.Text;

namespace WaveletTransform.Model
{
    public class WaveletsInfoModel
    {
        public int index { set; get; }
        public string familyName { set; get; }
        public string familyShortName { set; get; }
        public int type { set; get; }
        public string[] tabNums { set; get; }
        public string typNums { set; get; }
        public string file { set; get; }
        public int[] bounds { set; get; }
        /// <summary>
        /// 中心频率
        /// </summary>
        public double centerfc { set; get; }
        public List<WaveletsInfoModel> GetWaveletsInfos()
        {
            List<WaveletsInfoModel> infos = new List<WaveletsInfoModel>();
            //Haar（哈尔）
            infos.Add(new WaveletsInfoModel
            {
                index = 1,
                familyName = "Haar",
                familyShortName = "haar",
                type = 1,
                tabNums = new string[] { "no" },
                typNums = "no",
                file = "dbwavf",
                bounds = new int[0]
            });
            //Daubechies（多贝西）
            infos.Add(new WaveletsInfoModel
            {
                index = 2,
                familyName = "Daubechies",
                familyShortName = "db",
                type = 1,
                tabNums = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "**" },
                typNums = "integer",
                file = "dbwavf",
                bounds = new int[0]
            });
            //Symlets
            infos.Add(new WaveletsInfoModel
            {
                index = 3,
                familyName = "Symlets",
                familyShortName = "sym",
                type = 1,
                tabNums = new string[] { "2", "3", "4", "5", "6", "7", "8", "**" },
                typNums = "integer",
                file = "symwavf",
                bounds = new int[0]
            });
            //Coiflets
            infos.Add(new WaveletsInfoModel
            {
                index = 4,
                familyName = "Coiflets",
                familyShortName = "coif",
                type = 1,
                tabNums = new string[] { "1", "2", "3", "4", "5" },
                typNums = "integer",
                file = "coifwavf",
                bounds = new int[0]
            });
            //BiorSplines
            infos.Add(new WaveletsInfoModel
            {
                index = 5,
                familyName = "BiorSplines",
                familyShortName = "bior",
                type = 2,
                tabNums = new string[] { "1.1", "1.3", "1.5", "2.2", "2.4", "2.6", "2.8", "3.1", "3.3", "3.5", "3.7", "3.9", "4.4", "5.5", "6.8" },
                typNums = "real",
                file = "biorwavf",
                bounds = new int[0]
            });
            //ReverseBior
            infos.Add(new WaveletsInfoModel
            {
                index = 6,
                familyName = "ReverseBior",
                familyShortName = "rbio",
                type = 2,
                tabNums = new string[] { "1.1", "1.3", "1.5", "2.2", "2.4", "2.6", "2.8", "3.1", "3.3", "3.5", "3.7", "3.9", "4.4", "5.5", "6.8" },
                typNums = "real",
                file = "rbiowavf",
                bounds = new int[0]
            });
            //Meyer
            infos.Add(new WaveletsInfoModel
            {
                index = 7,
                familyName = "Meyer",
                familyShortName = "meyr",
                type = 3,
                tabNums = new string[] { "no" },
                typNums = "no",
                file = "meyer",
                bounds = new int[] { -8, 8 }
            });
            //DMeyer
            infos.Add(new WaveletsInfoModel
            {
                index = 8,
                familyName = "DMeyer",
                familyShortName = "dmey",
                type = 1,
                tabNums = new string[] { "no" },
                typNums = "no",
                file = "demy.mat",
                bounds = new int[0]
            });
            //Gaussian
            infos.Add(new WaveletsInfoModel
            {
                index = 9,
                familyName = "Gaussian",
                familyShortName = "gaus",
                type = 4,
                tabNums = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" },
                typNums = "integer",
                file = "gauswavf",
                bounds = new int[] { -5, 5 }
            });
            //Mexican_hat
            infos.Add(new WaveletsInfoModel
            {
                index = 10,
                familyName = "Mexican_hat",
                familyShortName = "mexh",
                type = 4,
                tabNums = new string[] { "no" },
                typNums = "no",
                file = "mexiwavf",
                bounds = new int[] { -8, 8 }
            });
            //Morlet
            infos.Add(new WaveletsInfoModel
            {
                index = 11,
                familyName = "Morlet",
                familyShortName = "morl",
                type = 4,
                tabNums = new string[] { "no" },
                typNums = "no",
                file = "morlet",
                bounds = new int[] { -8, 8 }
            });
            //ComplexGaussian
            infos.Add(new WaveletsInfoModel
            {
                index = 12,
                familyName = "ComplexGaussian",
                familyShortName = "cgau",
                type = 5,
                tabNums = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" },
                typNums = "integer",
                file = "cguswavf",
                bounds = new int[] { -5, 5 }
            });
            //Shannon
            infos.Add(new WaveletsInfoModel
            {
                index = 13,
                familyName = "Shannon",
                familyShortName = "shan",
                type = 5,
                tabNums = new string[] { "1-1.5", "1-1", "1-0.5", "1-0.1", "2-3", "**" },
                typNums = "string",
                file = "shanwavf",
                bounds = new int[] { -20, 20 }
            });
            //FrequencyB-Spline
            infos.Add(new WaveletsInfoModel
            {
                index = 14,
                familyName = "FrequencyB-Spline",
                familyShortName = "fbsp",
                type = 5,
                tabNums = new string[] { "1-1-1.5", "1-1-1", "1-1-0.5", "2-1-1", "2-1-0.5", "2-1-0.1", "**" },
                typNums = "string",
                file = "fbspwavf",
                bounds = new int[] { -20, 20 }
            });
            //ComplexMorlet
            infos.Add(new WaveletsInfoModel
            {
                index = 15,
                familyName = "ComplexMorlet",
                familyShortName = "cmor",
                type = 5,
                tabNums = new string[] { "1-1.5", "1-1", "1-0.5", "1-1", "1-0.5", "1-0.1", "**" },
                typNums = "string",
                file = "cmorwavf",
                bounds = new int[] { -8, 8 }
            });
            //Fejer-Korovkin
            infos.Add(new WaveletsInfoModel
            {
                index = 16,
                familyName = "ComplexMorlet",
                familyShortName = "fk",
                type = 1,
                tabNums = new string[] { "4", "6", "8", "14", "18", "22" },
                typNums = "integer",
                file = "fejerkorovkin",
                bounds = new int[0]
            });
            return infos;
        }
    }
}
