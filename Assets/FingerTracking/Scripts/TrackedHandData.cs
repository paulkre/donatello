namespace FingerTracking
{

    [System.Serializable]
    public class TrackedHandData
    {
        //public float[,] metaPositions = new float[5,3];
        //public float[,,] positions = new float[5, 4, 3];
        //public float[,] transformationMatrix = new float[4, 4];

        public int side;
        public float[,] segmentLengths = new float[5, 3];
        public float[,] positions = new float[22, 3];
    }

}
