using System.Threading.Tasks;
using MagicLeap.Android;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace WalkTheWorld.Controllers
{
    public class LocatorController : MonoBehaviour
    {
        [SerializeField]
        private GameObject lookDownIndicator;
        
        [SerializeField]
        private GameObject visuals;

        [SerializeField]
        private Material onFloorMaterial;
        
        [SerializeField]
        private Material offFloorMaterial;

        [SerializeField]
        private GameObject displayQuad;

        [SerializeField]
        private bool askForPermission = false;
        
        private SurfaceMagnetism surfaceMagnetism;

        protected virtual void Awake()
        {
            if (visuals == null && transform.childCount > 0) 
            {
                visuals = transform.GetChild(0).gameObject;
            }
        }

        private void Start()
        {
            surfaceMagnetism = visuals.GetComponent<SurfaceMagnetism>();
            visuals.SetActive(true);
            if (askForPermission)
            {
                Permissions.RequestPermissions(new[]
                    {
                        MLPermission.SpatialMapping
                    },
                    OnPermissionGranted, OnPermissionDenied, OnPermissionDenied);
            }
        }
        
        private void OnPermissionDenied(string permission)
        {
            Debug.Log("SpatialMappingPermissionDenied");
        }

        private void OnPermissionGranted(string permission)
        {
            Debug.Log("SpatialMappingPermissionApproved");
        }

        private float startTime;
        private void Update()
        {
            var horizontalDistance = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z) - new Vector3(visuals.transform.position.x, 0, visuals.transform.position.z);
            var onSurface = surfaceMagnetism.OnSurface &&
                            Mathf.Abs(Camera.main.transform.position.y - visuals.transform.position.y) > 1.1f && 
                            Mathf.Abs(horizontalDistance.magnitude) < 1f;
            
            if (!onSurface)
            {
                startTime = Time.time;
            }
            var foundFloor = Time.time - startTime > 2f;
            
            lookDownIndicator.SetActive(!onSurface);
            displayQuad.GetComponent<Renderer>().material = onSurface ? onFloorMaterial : offFloorMaterial;
        }
    }
}

