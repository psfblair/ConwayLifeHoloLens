namespace ConwayLife.Unity

open UnityEngine

open ConwayLife.Core

module UnityLifeUI = 

    let unityCoordinatesFrom (scalingFactor: float32) (cell: Life.Cell) =
        match cell with
        | (x, y, z) -> Vector3(float32 x * scalingFactor, float32 y * scalingFactor, float32 z * scalingFactor)

    let lifeUI (token: GameObject) (parent: Transform) (scalingFactor: float32) : LifeUI =
        { giveBirthAt = 
            fun cell -> 
                Object.Instantiate(token, (unityCoordinatesFrom scalingFactor cell), Quaternion.identity, parent) 
                |> ignore
        ; dieAt = 
            fun cell -> 
                let center = unityCoordinatesFrom scalingFactor cell 
                let radius = token.GetComponent<SphereCollider>().radius
                let collidersWithinOrTouchingSphere = Physics.OverlapSphere(center, radius)

                collidersWithinOrTouchingSphere 
                |> Array.iter (fun collider -> collider.gameObject |> Object.Destroy)
        }


