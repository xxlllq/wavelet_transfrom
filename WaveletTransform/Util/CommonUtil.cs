using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace WaveletTransform.Util
{
    /// <summary>
    /// 通用工具类
    /// </summary>
    public class CommonUtil
    {
        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyByBin<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        /// <summary>
        /// 获取二维数组指定rid行
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double[] GetRow(int rid, double[,] mat)
        {
            int cols = mat.GetLength(1);
            double[] row = new double[cols];
            for (int j = 0; j < cols; j++)
                row[j] = mat[rid, j];
            return row;
        }

        /// <summary>
        /// 设置二维数组中的指定行
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="row"></param>
        /// <param name="mat"></param>
        public static void SetRow(int rid, double[] row, double[,] mat)
        {
            int cols = mat.GetLength(1);
            for (int j = 0; j < cols; j++)
                mat[rid, j] = row[j];
        }

        /// <summary>
        /// 获取二维数组中的指定列
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double[] GetCol(int cid, double[,] mat)
        {
            int rows = mat.GetLength(0);
            double[] col = new double[rows];
            for (int i = 0; i < rows; i++)
                col[i] = mat[i, cid];
            return col;
        }

        /// <summary>
        /// 设置二维数组中的列
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="col"></param>
        /// <param name="mat"></param>
        public static void SetCol(int cid, double[] col, double[,] mat)
        {
            int rows = mat.GetLength(0);
            for (int i = 0; i < rows; i++)
                mat[i, cid] = col[i];
        }
    }
}
