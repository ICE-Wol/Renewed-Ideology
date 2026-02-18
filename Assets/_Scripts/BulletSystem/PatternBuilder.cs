using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using UnityEngine;
using _Scripts.Tools;

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
        track.bulletSegments.Add(new BeginFogSegment() {
            startTime = startTime,
            endTime = startTime + fogDuration,
            bulletType = bulletType,
        });
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
            var emmitorTrack = pattern.CreateEmitterTrack()
            .InitTrack(0f, 10f, BulletType.JadeM, Color.red, 0.3f)
            .PositionOnCircle(Vector2.zero, 1f, emmitorCount, i)
            .ChainedAccelerate()
                .Speed(5f, 5f + i / 5f, 0f, LerpType.Linear)
            .End()
            .RotationFollow(0f, 10f, 4f);

            for(int j = 0; j < 10; j++)
            {
                var emmitorPosition = pattern.bulletStateToIDMap[emmitorTrack.id].position;
                pattern.CreateTrack()
                .InitTrack(5f, 10f, BulletType.JadeS, Color.red, 0.3f)
                .PositionOnCircle(emmitorPosition, 1f, emmitorCount, i)
                .ChainedAccelerate()
                    .Speed(5f, 2f + j / 5f, 5f, LerpType.Linear)
                .End()
                .RotationFollow(5f, 10f, 4f)
                .HSVColorChange(5f, 10f, ColorPalette.orange, ColorPalette.lightYellow);
            }
        }
        return pattern;
    }
}