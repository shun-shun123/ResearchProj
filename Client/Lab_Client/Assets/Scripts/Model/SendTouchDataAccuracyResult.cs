using System;

namespace Model
{
    [Serializable]
    public class SendTouchDataAccuracyResult
    {
        /// <summary>
        /// 最大送信整数値
        /// </summary>
        public string TestMaxCount;

        /// <summary>
        /// タッチ間隔(millis)
        /// </summary>
        public string TestDuration;

        /// <summary>
        /// 誤認識した回数
        /// </summary>
        public string MissTouchCount;

        /// <summary>
        /// 正しく認識した回数
        /// </summary>
        public string CorrectTouchCount;

        /// <summary>
        /// 認識精度
        /// </summary>
        public string Accuracy;
    }
}