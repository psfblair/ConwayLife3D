namespace ConwayLife3D.Controllers

open UnityEngine

type CameraController() = 
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable speed: float32 = 2.0f

    [<SerializeField>] 
    let mutable rotationSpeed: float32 = 90.0f

    member this.Update () =

        (******* LEFT - RIGHT ***************************************************************)
        if Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)
        then this.transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f))

        if Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)
        then this.transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f))
     
        (******* FORWARD - BACK *************************************************************)
        if Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)
        then this.transform.Translate(new Vector3(0.0f, 0.0f, -speed * Time.deltaTime))
     
        if Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)
        then this.transform.Translate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime))
     
        (******* UP - DOWN ******************************************************************)
        if Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Return) 
        then this.transform.Translate(new Vector3(0.0f, -speed * Time.deltaTime, 0.0f))
     
        if Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightShift)
        then this.transform.Translate(new Vector3(0.0f, speed * Time.deltaTime, 0.0f))

        (******* ROTATION - YAW *************************************************************)
        if Input.GetKey(KeyCode.Comma)
        then this.transform.Rotate(new Vector3(0.0f, -rotationSpeed * Time.deltaTime, 0.0f))

        if Input.GetKey(KeyCode.Slash)
        then this.transform.Rotate(new Vector3(0.0f, rotationSpeed * Time.deltaTime, 0.0f))

        (******* ROTATION - PITCH ***********************************************************)
        if Input.GetKey(KeyCode.Period)
        then this.transform.Rotate(new Vector3(-rotationSpeed * Time.deltaTime, 0.0f, 0.0f))

        if Input.GetKey(KeyCode.L)
        then this.transform.Rotate(new Vector3(rotationSpeed * Time.deltaTime, 0.0f, 0.0f))
