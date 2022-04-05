using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAgent : MonoBehaviour
{
    [Header("Movement")]
    public float MaxSpeed = 10.0f;
    public float CurrentSpeed = 30.0f;

    [Header("Raycast")]
    public Vector3 OriginPoint;
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
        CenterOfOpenArea = Vector3.zero;
        pointCount = 0;

        // Para cada raio lançado no espaço
        for (int rayIndex = 0; rayIndex < RayQuantitie; rayIndex++) {
            // Um ângulo é atribuído (0 a 180)
            float angle = rayIndex * 180 / (RayQuantitie - 1);
            float angleInRad = angle * Mathf.PI / (float)180.0;

            // Uma direção é atribuída baseada no ângulo
            Vector3 direction = Vector3.forward * Mathf.Sin(angleInRad) + Vector3.right * Mathf.Cos(angleInRad);

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


        // 
    }
}



public class RaycastHitHandler
{
    public RaycastHit Hit;

    Color BeforeHitColor = Color.yellow;
    Color AfterHitColor = Color.blue;
    Color NoHitColor = Color.green;
    Color TooCloseHitColor = Color.red;

    public void Ray(Vector3 DirectionVector, CarAgent car) {
        Vector3 OriginPoint = car.OriginPoint + car.transform.position;

        if(Physics.Raycast(OriginPoint, DirectionVector, out Hit, car.MaxRayDistance, car.DefaultRaycastLayers)) {
                        
            Debug.DrawRay(OriginPoint, DirectionVector * Hit.distance, Hit.distance > car.TooCloseDistance ? BeforeHitColor : TooCloseHitColor);
            Debug.DrawRay(Hit.point, DirectionVector * (car.MaxRayDistance - Hit.distance) , AfterHitColor);

            car.CenterOfOpenArea += Hit.point;
            car.pointCount += 1;
        }
        else {

            Debug.DrawRay(OriginPoint, DirectionVector * car.MaxRayDistance, NoHitColor);

            car.CenterOfOpenArea += car.OriginPoint + DirectionVector * car.MaxRayDistance * car.OpenSpaceWheight;
            car.pointCount += car.OpenSpaceWheight;
        }
    }

}