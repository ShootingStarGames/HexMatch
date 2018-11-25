using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 10f; // 외각 반지름

    public const float innerRadius = outerRadius * 0.866025404f; // 내부 반지름

    private const float gapOuterRadius = outerRadius * .95f;
    private const float gapInnerRadius = innerRadius * .95f;

    public static Vector3[] corners = {
        new Vector3( gapOuterRadius           , 0f              , 0),
        new Vector3(  0.5f * gapOuterRadius   , gapInnerRadius  , 0),
        new Vector3( -0.5f * gapOuterRadius   , gapInnerRadius  , 0),
        new Vector3( -gapOuterRadius          , 0f              , 0),
        new Vector3( -0.5f * gapOuterRadius   , -gapInnerRadius , 0),
        new Vector3( 0.5f * gapOuterRadius    , -gapInnerRadius , 0),
        new Vector3( gapOuterRadius           , 0f              , 0)
    };  

    public const float uvOuterRadius = 0.5f; // 외각 반지름

    public const float uvInnerRadius = uvOuterRadius * 0.866025404f; // 내부 반지름

    public static Vector2[] uvCorners = {
        new Vector2( uvOuterRadius         , 0f              ),
        new Vector2(  0.5f * uvOuterRadius , uvInnerRadius   ),
        new Vector2( -0.5f * uvOuterRadius , uvInnerRadius   ),
        new Vector2( -uvOuterRadius        , 0f              ),
        new Vector2( -0.5f * uvOuterRadius , -uvInnerRadius  ),
        new Vector2( 0.5f * uvOuterRadius  , -uvInnerRadius  ),
        new Vector2( uvOuterRadius         , 0f              )
    };
}