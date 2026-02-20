using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class PatternBuilder
{
    public float cursorTime;

    public PatternTimeline pattern = new();

    public abstract PatternTimeline Build();

    public static Vector2 GetCirclePosition(float radius, float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
    }

    public BulletTrack Track(
        float startTime,
        float endTime,
        BulletType bulletType,
        float fogDuration)
    {
        var track = pattern.CreateTrack();
        track.spawnTime = startTime;
        track.despawnTime = endTime;
        track.bulletSegments.Add(new BeginFogSegment()
        {
            startTime = startTime,
            endTime = startTime + fogDuration,
            bulletType = bulletType,
        });
        cursorTime = track.spawnTime;
        return track;
    }

    public BulletTrack Emmitor(
        float startTime,
        float endTime)
    {
        var track = pattern.CreateEmitterTrack();
        track.spawnTime = startTime;
        track.despawnTime = endTime;
        cursorTime = track.spawnTime;
        return track;
    }
}

public static class PatternBuilderDSL
{
    public static BulletTrack InitTrack(this BulletTrack track, float startTime, float endTime, BulletType bulletType, Color color, float fogDuration)
    {
        track.spawnTime = startTime;
        track.despawnTime = endTime;
        track.bulletSegments.Add(new BeginFogSegment()
        {
            startTime = startTime,
            endTime = startTime + fogDuration,
            bulletType = bulletType,
            color = color,
        });
        return track;
    }

    public static BulletTrack InitTrack(this BulletTrack track, float startTime, float endTime, BulletType bulletType, float fogDuration)
    {
        track.spawnTime = startTime;
        track.despawnTime = endTime;
        track.bulletSegments.Add(new BeginFogSegment()
        {
            startTime = startTime,
            endTime = startTime + fogDuration,
            bulletType = bulletType,
        });
        return track;
    }
    
    public static BulletTrack SetLayer(this BulletTrack track, int layer)
    {
        track.layer = layer;
        return track;
    }
}

public class PatternBuilder_Test_0_0 : PatternBuilder
{
    public override PatternTimeline Build()
    {
        for (int j = 0; j < 40; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                float dir = i * 360f / 4f + j * 10f;
                var bulletSegments = new List<BulletSegment>
                {
                    new BeginFogSegment()
                    {
                        startTime = j / 8f,
                        endTime = j / 8f + 0.3f,
                        bulletType = BulletType.Scale,
                    },
                    new LinearAccelerateSegment()
                    {
                        startTime = j / 8f,
                        endTime = 5f + j / 8f,
                        speedChange = new Vector2(7f - j / 20f, 0f),
                        angleChange = new Vector2(dir, -dir / 2f),
                    },
                    new LinearAccelerateSegment()
                    {
                        startTime = 5f + j / 8f,
                        endTime = 10f + j / 8f,
                        speedChange = new Vector2(0f, 3f + j / 20f),
                        angleChange = new Vector2(-dir / 2f, dir),
                    },
                    new RgbColorChangeSegment()
                    {
                        startTime = j / 8f,
                        endTime = 10f + j / 8f,
                        colorStart = Color.Lerp(Color.cyan, Color.blue, j / 40f),
                        colorEnd = Color.Lerp(Color.green, ColorPalette.seaGreen, j / 40f),
                    },
                    new RotationFollowSegment()
                    {
                        startTime = j / 8f,
                        endTime = 10f + j / 8f,
                    }
                };
                var bulletTrack = new BulletTrack()
                {
                    spawnTime = j / 8f,
                    despawnTime = 10f + j / 8f,
                    startPosition = GetCirclePosition(1f, dir),
                    startRotation = dir,
                    bulletSegments = bulletSegments,
                };
                pattern.bulletTracks.Add(bulletTrack);
            }
        }
        return pattern;
    }
}

public class PatternBuilder_Test_0_0_1 : PatternBuilder
{
    public override PatternTimeline Build()
    {
        for (int j = 0; j < 40; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                float dir = i * 360f / 4f + j * 10f;
                float startTime = j / 8f;
                float endTime = 10f + j / 8f;
                pattern.CreateTrack()
                .InitTrack(startTime, endTime, BulletType.Scale, 0.3f)
                .PositionOnCircle(1f, i, 4)
                .ChainedAccelerate()
                    .Speed(5f, 7f - j / 20f, 0f, LerpType.Linear)
                    .Speed(5f, 0f, 3f + j / 20f, LerpType.Linear)
                    .Angle(5f, dir, -dir / 2f, LerpType.Linear)
                    .Angle(5f, -dir / 2f, dir, LerpType.Linear)
                .End()
                .RgbColorChange(startTime, endTime, Color.Lerp(Color.cyan, Color.blue, j / 40f), Color.Lerp(Color.green, ColorPalette.seaGreen, j / 40f))
                .RotationFollow(startTime, endTime, 4f);
            }
        }
        return pattern;
    }
}
public class PatternBuilder_Test_0_1 : PatternBuilder
{
    public override PatternTimeline Build()
    {
        int bulletCount = 24;
        for (int i = 0; i < bulletCount; i++)
        {
            float dir = i * 360f / bulletCount;

            var bulletTrack = new BulletTrack()
            {
                spawnTime = 0f,
                despawnTime = 0f + 10f,
                startPosition = GetCirclePosition(1f, i * 360f / bulletCount),
                startRotation = dir,
                bulletSegments = new List<BulletSegment>
                {
                    new BeginFogSegment()
                    {
                        startTime = 0f,
                        endTime = 0f + 0.3f,
                        bulletType = BulletType.JadeS,
                    },
                    new MarkForDestroySegment()
                    {
                        triggerTime = 5f,
                    },
                    new RotationFollowSegment()
                    {
                        startTime = 0f,
                        endTime = 0f + 10f,
                    },
                    new LinearAccelerateSegment()
                    {
                        startTime = 0f,
                        endTime = 0f + 10f,
                        speedChange = new Vector2(1f, 0f),
                        angleChange = new Vector2(dir, dir + 90f),
                    },
                },
            };

            int petalNum = 10;
            for (int j = 0; j < 10; j++)
            {
                float spawnTime = 1f + j / 10f;
                var bulletTrackSub = new BulletTrack()
                {
                    parentID = bulletTrack.id,
                    layer = 1,
                    spawnTime = spawnTime,
                    despawnTime = spawnTime + 10f,
                    startPosition = GetCirclePosition(0.5f, dir),
                    bulletSegments = new List<BulletSegment>
                    {
                        new BeginFogSegment()
                        {
                            startTime = spawnTime,
                            endTime = spawnTime + 0.2f,
                            bulletType = BulletType.Scale,
                        },
                        new FlowerSegment()
                        {
                            startTime = spawnTime,
                            endTime = spawnTime + 10f,
                            petalRadius = 0.5f,
                            petalCount = petalNum,
                            petalIndex = j,
                            petalAngleChange = new Vector2(0, 0),
                            fallBackSegment = new LinearAccelerateSegment()
                            {
                                startTime = spawnTime,
                                endTime = spawnTime + 10f,
                                speedChange = new Vector2(0f, 1f),
                                angleChange = new Vector2(0f, 0f),
                            },
                        },
                    },
                };
                pattern.bulletTracks.Add(bulletTrackSub);

            }
            pattern.bulletTracks.Add(bulletTrack);
        }
        return pattern;
    }
}

public class PatternBuilder_Test_MountainOfFaith : PatternBuilder
{
    public override PatternTimeline Build()
    {
        float l2 = 1f / Mathf.Sin(18f * Mathf.Deg2Rad);
        Vector2[] posEdge1 = new Vector2[5];
        Vector2[] posCenter1 = new Vector2[5];
        float[] oriDeg = new float[5];
        float totalDeg = (180f - 18f * 2f) * 2f;
        for (int i = 0; i < 5; i++)
        {
            float oriAngle = 90f + i * 360f / 5f;
            posEdge1[i] = GetCirclePosition(1f, oriAngle);
            posCenter1[i] = GetCirclePosition(l2 / 2f, oriAngle);
        }
        for (int i = 0; i < 5; i++)
        {
            var prei = (i + 5 - 1) % 5;
            oriDeg[i] = Vector2.SignedAngle(Vector2.right, posEdge1[prei] - posCenter1[i]);
        }
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                float angle = oriDeg[i] + j * totalDeg / 50f;
                float shootAngle = angle + 180f + j / 4 * 5f - i * 0.8f;

                pattern.CreateTrack()
                .InitTrack(j / 50f, 30f, BulletType.Scale, 0.3f)
                .PositionOnCircle(posCenter1[i], l2 / 2f, angle)
                .ChainedAccelerate()
                    .Speed(3f - j / 50f, 0f, 0f, LerpType.Linear)
                    .Speed(10f, 0f, 6f, LerpType.FInSOutCubic)
                    .Angle(20f, shootAngle, shootAngle, LerpType.Linear)
                .End()
                .RotationFollow(0f, 30f, 4f)
                .HSVColorChange(j / 50f, 30f, ColorPalette.orange, ColorPalette.lightYellow);
            }
        }

        Vector2[] posEdge2 = new Vector2[5];
        Vector2[] posCenter2 = new Vector2[5];
        float[] oriDeg2 = new float[5];
        float totalDeg2 = 180f;
        for (int i = 0; i < 5; i++)
        {
            float oriAngle = 90f + i * 360f / 5f;
            posEdge2[i] = GetCirclePosition(l2, oriAngle);
        }
        for (int i = 0; i < 5; i++)
        {
            var prei = (i + 5 - 1) % 5;
            posCenter2[i] = (posEdge2[i] + posEdge2[prei]) / 2f;
            oriDeg2[i] = Vector2.SignedAngle(Vector2.right, posEdge2[prei] - posCenter2[i]);
        }
        float radius2 = (posEdge2[1] - posEdge2[0]).magnitude / 2f;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                float angle = oriDeg2[i] + j * totalDeg2 / 50f;
                float shootAngle = angle + 180f + j / 4 * 5f - i * 0.8f;

                pattern.CreateTrack()
                .InitTrack(1f + j / 50f, 30f, BulletType.Scale, 0.3f)
                .PositionOnCircle(posCenter2[i], radius2, angle)
                    .ChainedAccelerate()
                        .Speed(4f - (1f + j / 50f), 0f, 0f, LerpType.Linear)
                        .Speed(15f, 0f, 6f, LerpType.FInSOutCubic)
                        .Angle(20f, shootAngle, shootAngle, LerpType.Linear)
                    .End()
                    .RotationFollow(0f, 30f, 4f)
                    .HSVColorChange(1f + j / 50f, 30f, ColorPalette.lightPink, ColorPalette.purple);
            }
        }

        Vector2[] posEdge3 = new Vector2[5];
        Vector2[] posCenter3 = new Vector2[5];
        float[] oriDeg3 = new float[5];
        float totalDeg3 = 180f;
        for (int i = 0; i < 5; i++)
        {
            float angle = oriDeg2[i] + totalDeg3 / 2f;
            posEdge3[i] = posCenter2[i] + GetCirclePosition(radius2, angle);
        }
        for (int i = 0; i < 5; i++)
        {
            var prei = (i + 5 - 1) % 5;
            posCenter3[i] = (posEdge3[i] + posEdge3[prei]) / 2f;
            oriDeg3[i] = Vector2.SignedAngle(Vector2.right, posEdge3[prei] - posCenter3[i]);
        }
        float radius3 = (posEdge3[1] - posEdge3[0]).magnitude / 2f;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                float angle = oriDeg3[i] + j * totalDeg3 / 50f;
                float shootAngle = angle + 180f + j / 4 * 5f - i * 0.8f;

                pattern.CreateTrack()
                .InitTrack(2f + j / 50f, 30f, BulletType.Scale, 0.3f)
                .PositionOnCircle(posCenter3[i], radius3, angle)
                .ChainedAccelerate()
                    .Speed(5f - (2f + j / 50f), 0f, 0f, LerpType.Linear)
                    .Speed(20f, 0f, 6f, LerpType.FInSOutCubic)
                    .Angle(20f, shootAngle, shootAngle, LerpType.Linear)
                .End()
                .RotationFollow(0f, 30f, 4f)
                .HSVColorChange(2f + j / 50f, 30f, ColorPalette.lightBlue, Color.cyan);
            }
        }

        return pattern;
    }
}

public class PatternBuilder_Test_0_2 : PatternBuilder
{
    public override PatternTimeline Build()
    {
        int emmitorCount = 10;
        for (int i = 0; i < emmitorCount; i++)
        {
            float angle = i * 360f / emmitorCount;
            var emmitorTrack = pattern.CreateTrack()
            .InitTrack(0f, 2f, BulletType.JadeM, Color.red, 0.3f)
            .PositionOnCircle(new Vector2(0f, 1f), 0.5f, i, emmitorCount)
            .ChainedAccelerate()
                .Speed(Random.Range(1f, 2f), 0f, LerpType.FInSOutCubic)
                .Angle(angle, angle, LerpType.Linear)
            .End()
            .RotationFollow(0f, 10f, 4f);

            float innerCount = 20;
            for (int j = 0; j < innerCount; j++)
            {
                float innerAngle = i * 360f / emmitorCount + j * 360f / innerCount;
                pattern.CreateTrack()
                .InitTrack(2f, 10f, BulletType.JadeS, Color.red, 0.3f)
                .SetLayer(1)
                .CirclePositionFromRefBullet(emmitorTrack, 2f, innerAngle, 0.5f)
                .ChainedAccelerate()
                    .Speed(2f, 0f, 0f, LerpType.Linear)
                    .Speed(5f, 3f, 2f, LerpType.Linear)
                    .Angle(innerAngle, innerAngle, LerpType.Linear)
                .End()
                .RotationFollow(2f, 10f, 4f)
                .HSVColorChange(2f, 10f, Color.red, ColorPalette.lightYellow);
            }
        }
        return pattern;
    }
}


public class PatternBuilder_Test_0_3 : PatternBuilder
{
    public override PatternTimeline Build()
    {
        int bulletWays = 20;
        int bulletCount = 10;
        for (int j = 0; j < bulletCount; j++)
        {
            for (int i = 0; i < bulletWays; i++)
            {
                float initAngle = i * 360f / bulletWays;
                float offsetAngle = j * 3f;
                float angle = initAngle + offsetAngle;
                float offsetTime = j / (float)bulletCount * 1.5f;
                var bulletTrack = pattern.CreateTrack()
                .InitTrack(0f + offsetTime, 0.8f + offsetTime, BulletType.Bacteria, Color.green, 0.3f)
                .PositionOnCircle(new Vector2(0f, 2f), 0.5f, offsetAngle, i, bulletWays)
                .ChainedAccelerate()
                    .Speed(1f, 2f, LerpType.FInSOutCubic)
                    .Angle(angle)
                .End()
                .RotationFollow(0f, 10f, 4f);

                var bulletTrackSub = pattern.CreateTrack()
                .InitTrack(0.8f + offsetTime, 1.6f + offsetTime, BulletType.Rice, Color.green, 0.3f)
                .SetLayer(1)
                .ExtendFromRefBullet(bulletTrack, 0.8f + offsetTime)
                .ChainedAccelerate()
                    .Speed(2f, 3f, LerpType.FInSOutCubic)
                    .Angle(angle)
                .End()
                .RotationFollow(4f);

                var bulletTrackSubSub = pattern.CreateTrack()
                .InitTrack(1.6f + offsetTime, 10f + offsetTime, BulletType.Bacteria, Color.green, 0.3f)
                .SetLayer(2)
                .ExtendFromRefBullet(bulletTrackSub, 1.6f + offsetTime)
                .ChainedAccelerate()
                    .Speed(1f, 3f, 1f, LerpType.FInSOutCubic)
                    .Angle(angle)
                .End()
                .RotationFollow(4f);

            }
        }

        return pattern;
    }
}

public class PatternBuilder_Test_0_3_1 : PatternBuilder
{
    public override PatternTimeline Build()
    {
        int bulletWays = 15;
        int bulletCount = 20;
        for (int j = 0; j < bulletCount; j++)
        {
            for (int i = 0; i < bulletWays; i++)
            {
                float initAngle = i * 360f / bulletWays;
                float offsetAngle = j * 1.5f;
                float angle = initAngle + offsetAngle;
                float stage2OffsetAngle = j * 10f;
                float offsetTime = j / (float)bulletCount * 1f;

                Color endColor = Calc.LerpColorInHSV(Color.cyan, Color.orange, j / (float)bulletCount);

                var bulletTrack = pattern.CreateTrack()
                .InitTrack(0f + offsetTime, 20f + offsetTime, BulletType.Bacteria, Color.lightBlue, 0.3f)
                .PositionOnCircle(new Vector2(0f, 2f), 0.5f, offsetAngle, i, bulletWays)
                .ChainedAccelerate()
                    .Speed(0.8f, 3f, 0.5f, LerpType.SInFOutCubic)
                    .Speed(0.8f, 0.5f, 3f, LerpType.FInSOutCubic)
                    .Speed(0.8f, 3f, 1f, LerpType.FInSOutCubic)
                    .Speed(10f, 1f, 3f, LerpType.FInSOutCubic)
                    .Angle(0.8f, angle, angle, LerpType.Linear)
                    .Angle(0.8f, angle, angle + stage2OffsetAngle, LerpType.Linear)
                    .Angle(0.8f, angle + stage2OffsetAngle, angle, LerpType.FInOutSMid)
                //.Angle(2f, angle, angle + 180f - stage2OffsetAngle * 2f, LerpType.FInOutSMid)
                .End()
                .BeginFog(0.8f + offsetTime, 0.3f, BulletType.Rice, Color.cyan)
                .BeginFog(1.6f + offsetTime, 0.3f, BulletType.Bacteria, endColor)
                .BeginFog(2.4f + offsetTime, 0.3f, BulletType.Rice, Color.lightBlue)
                .RotationFollow(0f, 10f, 4f)
                .HSVColorChangeDuration(0.8f + offsetTime, 0.8f, Color.cyan, endColor);


                initAngle = i * 360f / bulletWays;
                offsetAngle = -j * 1.5f;
                angle = initAngle + offsetAngle;
                offsetTime = j / (float)bulletCount * 1f;
                bulletTrack = pattern.CreateTrack()
                .InitTrack(0f + offsetTime, 20f + offsetTime, BulletType.Bacteria, Color.lightBlue, 0.3f)
                .PositionOnCircle(new Vector2(0f, 2f), 0.5f, offsetAngle, i, bulletWays)
                .ChainedAccelerate()
                    .Speed(0.8f, 3f, 0.5f, LerpType.SInFOutCubic)
                    .Speed(0.8f, 0.5f, 3f, LerpType.FInSOutCubic)
                    .Speed(0.8f, 3f, 1f, LerpType.FInSOutCubic)
                    .Speed(10f, 1f, 3f, LerpType.FInSOutCubic)
                    .Angle(0.8f, angle, angle, LerpType.Linear)
                    .Angle(0.8f, angle, angle - stage2OffsetAngle, LerpType.Linear)
                    .Angle(0.8f, angle - stage2OffsetAngle, angle, LerpType.FInOutSMid)
                //.Angle(2f, angle, angle + 180f - stage2OffsetAngle * 2f, LerpType.FInOutSMid)
                .End()
                .BeginFog(0.8f + offsetTime, 0.3f, BulletType.Rice, Color.cyan)
                .BeginFog(1.6f + offsetTime, 0.3f, BulletType.Bacteria, endColor)
                .BeginFog(2.4f + offsetTime, 0.3f, BulletType.Rice, Color.lightBlue)
                .RotationFollow(0f, 10f, 4f)
                .HSVColorChangeDuration(0.8f + offsetTime, 0.8f, Color.cyan, endColor);

            }
        }

        return pattern;
    }
}

public class PatternBuilder_Test_0_3_2 : PatternBuilder
{
    public override PatternTimeline Build()
    {
        int bulletWays = 15;
        int bulletCount = 20;
        for (int j = 0; j < bulletCount; j++)
        {
            for (int i = 0; i < bulletWays; i++)
            {
                float initAngle = i * 360f / bulletWays;
                float offsetAngle = j * 1.5f;
                float angle = initAngle + offsetAngle;
                float stage2OffsetAngle = j * 10f;
                float offsetTime = j / (float)bulletCount * 1f;

                Color midColor = Calc.LerpColorInHSV(Color.cyan, Color.orange, j / (float)bulletCount);
                Color endColor = Calc.LerpColorInHSV(Color.lightBlue, Color.purple, j / (float)bulletCount);

                var bulletTrack = pattern.CreateTrack()
                .InitTrack(0f + offsetTime, 20f + offsetTime, BulletType.Bacteria, Color.lightBlue, 0.3f)
                .PositionOnCircle(new Vector2(0f, 2f), 0.5f, offsetAngle, i, bulletWays)
                .ChainedAccelerate()
                    .Speed(0.8f, 3f, 0.5f, LerpType.SInFOutCubic)
                    .Speed(0.8f, 0.5f, 3f, LerpType.FInSOutCubic)
                    .Speed(0.8f, 3f, 1f, LerpType.FInSOutCubic)
                    .Speed(10f, 1f, 3f, LerpType.FInSOutCubic)
                    .Angle(0.8f, angle, angle, LerpType.Linear)
                    .Angle(0.8f, angle + 90f, angle + stage2OffsetAngle + 90f, LerpType.Linear)
                    .Angle(0.8f, angle + stage2OffsetAngle + 90f, angle + 90f, LerpType.FInOutSMid)
                    .Angle(5f, angle + 90f, angle + 90f - (180f - stage2OffsetAngle * 2f), LerpType.FInOutSMid)
                .End()
                .BeginFog(0.8f + offsetTime, 0.3f, BulletType.Rice, Color.cyan)
                .BeginFog(1.6f + offsetTime, 0.3f, BulletType.Bacteria, midColor)
                .BeginFog(2.4f + offsetTime, 0.3f, BulletType.Rice, Color.lightBlue)
                .RotationFollow(0f, 10f, 4f)
                .HSVColorChangeDuration(0.8f + offsetTime, 0.8f, Color.cyan, midColor)
                .HSVColorChangeDuration(2.4f + offsetTime, 1f, Color.lightBlue, endColor);


                initAngle = i * 360f / bulletWays;
                offsetAngle = -j * 1.5f;
                angle = initAngle + offsetAngle;
                offsetTime = j / (float)bulletCount * 1f;
                bulletTrack = pattern.CreateTrack()
                .InitTrack(0f + offsetTime, 20f + offsetTime, BulletType.Bacteria, Color.lightBlue, 0.3f)
                .PositionOnCircle(new Vector2(0f, 2f), 0.5f, offsetAngle, i, bulletWays)
                .ChainedAccelerate()
                    .Speed(0.8f, 3f, 0.5f, LerpType.SInFOutCubic)
                    .Speed(0.8f, 0.5f, 3f, LerpType.FInSOutCubic)
                    .Speed(0.8f, 3f, 1f, LerpType.FInSOutCubic)
                    .Speed(10f, 1f, 3f, LerpType.FInSOutCubic)
                    .Angle(0.8f, angle, angle, LerpType.Linear)
                    .Angle(0.8f, angle - 90f, angle - stage2OffsetAngle - 90f, LerpType.Linear)
                    .Angle(0.8f, angle - stage2OffsetAngle - 90f, angle - 90f, LerpType.FInOutSMid)
                    .Angle(5f, angle - 90f, angle - 90f + (180f - stage2OffsetAngle * 2f), LerpType.FInOutSMid)
                .End()
                .BeginFog(0.8f + offsetTime, 0.3f, BulletType.Rice, Color.cyan)
                .BeginFog(1.6f + offsetTime, 0.3f, BulletType.Bacteria, midColor)
                .BeginFog(2.4f + offsetTime, 0.3f, BulletType.Rice, Color.lightBlue)
                .RotationFollow(0f, 10f, 4f)
                .HSVColorChangeDuration(0.8f + offsetTime, 0.8f, Color.cyan, midColor)
                .HSVColorChangeDuration(2.4f + offsetTime, 1f, Color.lightBlue, endColor);

            }
        }

        return pattern;
    }
}