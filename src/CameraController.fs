namespace ConwayLife3D.Controllers

open UnityEngine

type CameraController() = 
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable speed: float32 = 1.0f

    [<SerializeField>] 
    let mutable rotationSpeed: float32 = 180.0f

    member this.Update () =

        let controlKeyPressed () = Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)
        let altKeyPressed () = Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt)

        (******* LEFT - RIGHT ***************************************************************)
        if Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)
        then this.transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f))

        if Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)
        then this.transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f))
     
        (******* UP - DOWN ******************************************************************)
        if Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)
        then this.transform.Translate(new Vector3(0.0f, speed * Time.deltaTime, 0.0f))
     
        if Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)
        then this.transform.Translate(new Vector3(0.0f, -speed * Time.deltaTime, 0.0f))
     
        (******* FORWARD - BACK *************************************************************)
        if controlKeyPressed () && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        then this.transform.Translate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime))
     
        if controlKeyPressed () && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        then this.transform.Translate(new Vector3(0.0f, 0.0f, -speed * Time.deltaTime))

        (******* ROTATION -YAW **************************************************************)
        if Input.GetKey(KeyCode.Comma)
        then this.transform.Rotate(new Vector3(0.0f, -rotationSpeed * Time.deltaTime, 0.0f))

        if Input.GetKey(KeyCode.Slash)
        then this.transform.Rotate(new Vector3(0.0f, rotationSpeed * Time.deltaTime, 0.0f))

        (******* ROTATION - PITCH ***********************************************************)
        if Input.GetKey(KeyCode.Period)
        then this.transform.Rotate(new Vector3(-rotationSpeed * Time.deltaTime, 0.0f, 0.0f))

        if Input.GetKey(KeyCode.L)
        then this.transform.Rotate(new Vector3(rotationSpeed * Time.deltaTime, 0.0f, 0.0f))

        (******* ROTATION - ROLL ************************************************************)
        if Input.GetKey(KeyCode.Z)
        then this.transform.Rotate(new Vector3(0.0f, 0.0f, -rotationSpeed * Time.deltaTime))

        if Input.GetKey(KeyCode.C)
        then this.transform.Rotate(new Vector3(0.0f, 0.0f, rotationSpeed * Time.deltaTime))

