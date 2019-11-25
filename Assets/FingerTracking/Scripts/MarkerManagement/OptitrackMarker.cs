using UnityEngine;

namespace FingerTracking.MarkerManagement
{

    public class OptitrackMarker
    {
        private Vector3[] positions;

        private const int positionLookback = 10;
        private int currentPositionIndex = 0;

        public float assignQuality = 0;
        public bool assignedInLastFrame = false;

        public OptitrackMarker(Vector3 position)
        {
            positions = new Vector3[positionLookback];
            for (int i = 0; i < positionLookback; i++)
            {
                positions[i] = position;
            }
        }

        OptitrackMarker parentMarker;
        Vector3 parentPosition;

        public void ClearParent()
        {
            parentMarker = null;
        }

        public void SetParent(OptitrackMarker parentMarker)
        {
            this.parentMarker = parentMarker;
        }

        public void SetParent(Vector3 parentPosition)
        {
            this.parentPosition = parentPosition;
        }


        public Quaternion orientation;

        private Vector3 predictPosition;
        private Vector3 predictMovement;
        private Vector3 predictAcceleration;
        float rating;

        public float GetRatingForMatch(Vector3 testPosition)
        {
            rating = (GetPredictedPosition() - testPosition).magnitude;

            float r = (GetCurrentPosition() - testPosition).magnitude;

            return rating;

            if (r < rating)
            {
                return r;
            }
            else
            {
                return rating;
            }
        }

        float decay = 0.9f;
        float limit = 0.01f;

        public Vector3 GetPredictedPosition()
        {
            predictAcceleration =
                (GetPosition(currentPositionIndex) - GetPosition(currentPositionIndex - 1)) -
                (GetPosition(currentPositionIndex - 2) - GetPosition(currentPositionIndex - 3));
            predictMovement = (GetPosition(currentPositionIndex) - GetPosition(currentPositionIndex - 1));// + predictAcceleration);
            predictMovement *= decay;

            if (predictMovement.magnitude > limit)
                predictMovement = predictMovement.normalized * limit;


            predictPosition = GetPosition(currentPositionIndex) + predictMovement;

            //predictPosition = GetCurrentPosition(); //maybe use to ignore prediction

            return predictPosition;
        }

        public Vector3 GetLastPosition()
        {
            return GetPosition(currentPositionIndex - 1);
        }


        public void CalibrationInit(Vector3 position)
        {
            for (int i = 0; i < positionLookback; i++)
            {
                positions[i] = position;
            }
        }

        private Vector3 GetPosition(int index)
        {
            //get position from array with underrun protection
            if (index < 0)
            {
                return positions[positionLookback + index];
            }
            else
            {
                return positions[index];
            }
        }

        public Vector3 GetParentPosition()
        {
            if (parentMarker != null)
            {
                return parentMarker.GetCurrentPosition();
            }
            else
            {
                return parentPosition;
            }

        }

        public void UpdatePosition(Vector3 newPosition, float quality)
        {
            if (Vector3.Distance(newPosition, GetCurrentPosition()) == 0)
            {
                //Debug.LogWarning("no update");
                return;
            }

            Vector3 up2 = Vector3.Cross(GetCurrentPosition(), Vector3.right);

            orientation = Quaternion.LookRotation(GetCurrentPosition() - GetParentPosition(), up2);

            //go to next free position in array
            currentPositionIndex++;
            if (currentPositionIndex == positionLookback)
            {
                currentPositionIndex = 0;
            }

            //overwrite entry
            positions[currentPositionIndex] = newPosition;

            //assign quality for debugging
            assignQuality = quality;
        }

        public Vector3 GetCurrentPosition()
        {
            try
            {
                return positions[currentPositionIndex];

            }
            catch (System.Exception e)
            {
                Debug.LogError($"error getcurrentposition: {currentPositionIndex}, {positions.Length} \n{e}");
                return Vector3.zero;
            }
        }

    }

}