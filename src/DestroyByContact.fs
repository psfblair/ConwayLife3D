namespace ConwayLife3D.Controllers

open UnityEngine

type DestroyByContact() =
    inherit MonoBehaviour()

    member this.OnTriggerEnter (other: Collider) = 
        Object.Destroy(other.gameObject)
        Object.Destroy(this.gameObject)

