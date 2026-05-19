using UnityEngine;

public class TurretController : MonoBehaviour {
    [SerializeField] private Transform selfTransform;
    [SerializeField] private Transform canonTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float detectionInnerRadius = 1f;
    [SerializeField] private float detectionOuterRadius = 3f;
    [SerializeField] private float turnSpeed = 1f;

    private enum DetectionStatus {
        TooClose,
        TooFar,
        Detected
    }

    private DetectionStatus GetDetectionStatus() {
        float distance = (targetTransform.position - selfTransform.position).magnitude;
        if (distance < detectionInnerRadius)
            return DetectionStatus.TooClose;
        else if (distance > detectionOuterRadius)
            return DetectionStatus.TooFar;
        else
            return DetectionStatus.Detected;
    }

    private void OnDrawGizmos() { // Reponse question 1
        Color outerColor = GetDetectionStatus() switch
        {
            DetectionStatus.TooClose => Color.yellow,
            DetectionStatus.TooFar => Color.red,
            DetectionStatus.Detected => Color.green,
        };
        outerColor.a = 0.5f;
        Gizmos.color = outerColor;
        Gizmos.DrawSphere(selfTransform.position, detectionOuterRadius);

        Color innerColor = Color.grey;
        innerColor.a = 0.6f;
        Gizmos.color = innerColor;
        Gizmos.DrawSphere(selfTransform.position, detectionInnerRadius);
    }

    private void Update() {
        if (GetDetectionStatus() == DetectionStatus.Detected) {
            float desiredAngle = CalculateDesiredAngleInDegrees();
            UpdateAimAngle(desiredAngle);
        }
    }

    private float CalculateDesiredAngleInDegrees() { // Reponse question 2
        Vector2 direction = targetTransform.position - selfTransform.position;
        float aimAngleInRadians = Mathf.Atan2(direction.y, direction.x);
        float aimAngleInDegrees = aimAngleInRadians / (2f * Mathf.PI) * 360f;
        return aimAngleInDegrees - 90f; // -90° car le sprite vise vers le haut par défaut plutôt que la droite
    }

    private void UpdateAimAngle(float desiredAngle) { // Reponse question 3
        float deltaAngle = Mathf.DeltaAngle(
                canonTransform.rotation.eulerAngles.z,
                desiredAngle); // Ecart entre l'angle actuel et l'angle désiré

        float newAngle;

        if (Mathf.Abs(deltaAngle) > 0.05f) { // S'il y a un écart supérieur à 0.05 degrés
            float directionSign = Mathf.Sign(deltaAngle);
            newAngle = canonTransform.rotation.eulerAngles.z + directionSign * turnSpeed;
            if (Mathf.Sign(
                    Mathf.DeltaAngle(
                    newAngle,
                    desiredAngle))
                * directionSign <= 0f) { // Si cette condition est vraie alors on a dépassé l'angle désiré
                newAngle = desiredAngle;
            }
        } else {
            newAngle = desiredAngle;
        }

        canonTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    private void OnValidate() {
        if (detectionInnerRadius > detectionOuterRadius) {
            detectionInnerRadius = detectionOuterRadius;
        }
    }
}
