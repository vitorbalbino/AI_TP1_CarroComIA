using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAgent : MonoBehaviour {
    [Header("Movement")]
    public float MaxSpeed = 10.0f;
    public float MaxAceleration = 10.0f;
    public Vector3 CurrentSpeed = Vector3.zero;
    public float Mass = 1.0f;
    [HideInInspector] public List<Vector3> Barriers = new List<Vector3>();

    [Header("Raycast")]
    public Vector3 OriginPoint;
    public float OriginPointExtension = 2.0f;
    public float MaxRayDistance = 12.0f;
    public float TooCloseDistance = 1.5f;
    [Range(5, 30)] public int RayQuantitie = 11;
    public LayerMask DefaultRaycastLayers;
    public float OpenSpaceWheight = 3;

    [HideInInspector] public Vector3 CenterOfOpenArea = Vector3.zero;
    [HideInInspector] public float pointCount = 0;

    RaycastHitHandler HitHandler = new RaycastHitHandler();



    void Update()// Rodando uma vez para testes
    {
        OriginPoint = this.transform.position + (this.transform.forward * OriginPointExtension) + (Vector3.up * 0.5f);
        CenterOfOpenArea = Vector3.zero;
        pointCount = 0;
        Barriers = new List<Vector3>();

        // Para cada raio lançado no espaço
        for(int rayIndex = 0; rayIndex < RayQuantitie; rayIndex++) {
            // Um ângulo é atribuído (0 a 180)
            float angle = rayIndex * 180 / (RayQuantitie - 1);
            float angleInRad = angle * Mathf.PI / (float)180.0;

            // Uma direção é atribuída baseada no ângulo
            Vector3 direction = this.transform.forward * Mathf.Sin(angleInRad) + this.transform.right * Mathf.Cos(angleInRad);

            // Um raycast é lançado para cada ângulo
            HitHandler.Ray(direction, this);
        }


        // Encontrar o centro da área aberta
        CenterOfOpenArea /= pointCount;

        // Draw a yellow sphere at the transform's position
        Debug.DrawRay(CenterOfOpenArea, Vector3.left * 0.1f, Color.white);
        Debug.DrawRay(CenterOfOpenArea, Vector3.forward * 0.1f, Color.white);
        /*
        
        Se um raycast não colidir, é preciso usar a maior distância observada
        Pegar o min / max em X pra tirar a média, dps o min / max em Y
        Contornar todos os pontos abaixo da distãncia de "avoidance"
        Fazer o carro dirigir na rota ideal


         */


        // Atualizar posição

        MovementHandler.Seek_Arrival(this);
    }
}



public class RaycastHitHandler {
    public RaycastHit Hit;

    Color BeforeHitColor = Color.yellow;
    Color AfterHitColor = Color.blue;
    Color NoHitColor = Color.green;
    Color TooCloseHitColor = Color.red;

    public void Ray(Vector3 DirectionVector, CarAgent car) {
        Vector3 OriginPoint = car.OriginPoint;

        if(Physics.Raycast(OriginPoint, DirectionVector, out Hit, car.MaxRayDistance, car.DefaultRaycastLayers)) {

            Debug.DrawRay(OriginPoint, DirectionVector * Hit.distance, Hit.distance > car.TooCloseDistance ? BeforeHitColor : TooCloseHitColor);
            Debug.DrawRay(Hit.point, DirectionVector * (car.MaxRayDistance - Hit.distance), AfterHitColor);

            car.CenterOfOpenArea += Hit.point;
            car.pointCount += 1;

            if ((Hit.point - car.transform.position).magnitude <= car.TooCloseDistance) {
                car.Barriers.Add(Hit.point);
            }
        }
        else {

            Debug.DrawRay(OriginPoint, DirectionVector * car.MaxRayDistance, NoHitColor);

            car.CenterOfOpenArea += car.OriginPoint + DirectionVector * car.MaxRayDistance * car.OpenSpaceWheight;
            car.pointCount += car.OpenSpaceWheight;
        }
    }

}



public static class MovementHandler {
    public static Vector3 TruncateInMagnitude(this Vector3 value, float Limit) {
        return (value.magnitude > Limit ? value.normalized * Limit : value);
    }

    public static Vector3 GetHeight(this Vector3 value, MonoBehaviour Reference) {
        value.y = Reference.transform.position.y;

        return value;
    }

    public static void SlowingVelocityToward(CarAgent car, Vector3 position) {
        float distanceMagnitude = (position - car.transform.position).magnitude;

        if(distanceMagnitude <= car.TooCloseDistance) {

            // Trunca a velocidade na direção do limite
            Vector3 VelocityToOBject = Vector3.Project(car.CurrentSpeed, (position - car.transform.position));

            // Preserva a velocidade perpendicular ao limite
            Vector3 OrtogonalVelocity = car.CurrentSpeed - VelocityToOBject;

            VelocityToOBject = TruncateInMagnitude(VelocityToOBject, 
                car.MaxSpeed * (distanceMagnitude / car.TooCloseDistance) * (VelocityToOBject.magnitude / car.CurrentSpeed.magnitude) );

            car.CurrentSpeed = VelocityToOBject + OrtogonalVelocity;
        }
    }

    public static void Seek_Arrival(CarAgent car) {
        Vector3 desired_vel = (car.CenterOfOpenArea.GetHeight(car) - car.transform.position).normalized * car.MaxSpeed;

        Vector3 steeringForce = desired_vel - car.CurrentSpeed;

        steeringForce = TruncateInMagnitude(steeringForce, car.MaxAceleration);

        Vector3 acelleration = steeringForce / car.Mass;

        car.CurrentSpeed = TruncateInMagnitude(car.CurrentSpeed + acelleration, car.MaxSpeed);

        // Arrival pro ponto ideal, e tenta evitar colisões {

        SlowingVelocityToward(car, car.CenterOfOpenArea.GetHeight(car));

        foreach(Vector3 point in car.Barriers) {
            SlowingVelocityToward(car, point);
        }

        // }

        Vector3 desiredPosition = car.transform.position + car.CurrentSpeed * Time.deltaTime;

        car.transform.LookAt(desiredPosition);

        car.transform.position = desiredPosition;

        Debug.Log($"CurrentSpeed = {car.CurrentSpeed};  acelleration = { acelleration}; desiredPosition = {desiredPosition}");
    }
}