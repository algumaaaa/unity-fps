using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
  private float stress = 0f;
  private Vector3 initialRot;

  private const float SHAKE_INTENSITY = 20f;
  private const float STRESS_DECAY = 1f;
  private const float MAX_SHAKE_X = 5f;
  private const float MAX_SHAKE_Y = 5f;
  private const float MAX_SHAKE_Z = 2.5f;

  private void Start()
  {
    initialRot = transform.localEulerAngles;
  }

  private void HandleCameraShake()
  {
    stress = Mathf.Max(stress - Time.deltaTime * STRESS_DECAY, 0f);
    float stress_sq = stress * stress;
    float px = initialRot.x + MAX_SHAKE_X * stress_sq * Random.Range(-SHAKE_INTENSITY, SHAKE_INTENSITY) * Time.deltaTime;
    float py = initialRot.y + MAX_SHAKE_Y * stress_sq * Random.Range(-SHAKE_INTENSITY, SHAKE_INTENSITY) * Time.deltaTime;
    float pz = initialRot.z + MAX_SHAKE_Z * stress_sq * Random.Range(-SHAKE_INTENSITY, SHAKE_INTENSITY) * Time.deltaTime;
    transform.Rotate(px, py, pz);
  }

  public void AddStress(float value)
  {
    stress = Mathf.Clamp(stress + value, 0f, 1f);
  }

  private void LateUpdate()
  {
    if (stress > 0f) HandleCameraShake();
  }
}
