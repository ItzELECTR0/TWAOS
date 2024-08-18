using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    internal static class TouchStickImageStick
    {
        private static string Raw => "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAACXBIWXMAABYlAAAWJQFJUiTwAAADqUlEQVR4nO3bzWtcVRjH8c+9iy6k6YsoKL5EfG0VROzGYLsvunBju2iV1mKpG427/gkKpojFTUGqRczCrFyo3UtxpYuCpgmp0IooJLSJ6Souroszw0wnM5mZe+/cM5npF7LIzeTc3/NwzplznpckyzID5hFMYT/24Vk8hN2YqH1mHWv4B4u4hnn8jL8GKS4ZkAMO4BheE4wuwjX8gFn8UnCsTZTpgAmcwTt4vqxBW/gdX+KCMGsKU4YD9mAaH+D+ooP1yC2cx2dYLTRSlmV5f5Isy05mWbacxWO5piHJctqRdwY8ja/waiHvl8cVnMD1fv8xzfGyI8JmNCzGE7T8Kmjri34ckGAG32JXvy+qgF2CthlBa0/0ugR2CLvvsVzSqmdW+Dba6PbBXhywA9/hcHFdlXIZb+jihG5LIMFF2894guaLuiyHbg74CMfLUhSB4/h4qw9stQTexFzZiiJxVAdbOjngSeFrZfcARVXJGl7GH61/aLcEElwyOsYTbLmkzX7QzgEncHDQiiJwECdbH7YugT3CffzBSiRVz7IQj1itP2idAdNG13iCbdPND5pnwE7cUN2VNha38IRaPKF5Brxn9I0n2Him/kt9BiRYwDORRFXNkrAXZPUZMGV8jCfEM6ZoLIG+79EjwFEaDtiOl52iHCbsAY/iz8hiYvF4ildiq4jIVIqXYquIyIupkLIaV/anmIytIiKTKR6OrSIiD6QaGdpxZGeS5UwNjQgbeTJDI0WqpDTzNmV93B1wJ8XfsVVEZCUVokDjyo1UKEYaV+ZTXI2tIiJXkyzLHsPN2EoiMZkKsYCF2EoisIib9YPQ5ZhKIvEjjZDYqGSB+2GOu8Pii0K0dBzYFBbPhOrLceGCYPNdqbEJ4VC0N5KoqrgtBIE2pcbWhdLTUee8pvvPOKbH9wkJUmxOj6/ibIWCquasJuNpXyOU4CfDVQpbBldwSG3zq9OpSOopoR54VOqE1oQmjk3F1J1CYtdxepCKKua0DpXkW8UE5/DJQORUy4wtTrrdaoUTfG37Vot+g7e1rPtm7hVL9zDQRm2g2RJEVcWsHoyn94aJDbyFcwVEVcU5QWtX48nXNXYEXxi+rpF/8a4+r/b3mqZyvnBJOFWdwkrOMcpgpabhkBzGU17j5Id4X7WNk5/jUwUbJwfROnvK4KpO5oU2mKFqnW3HAeEA8rriYbYlfC8cyIa6eboT9fb5F/CcRvv8XtyH/3BHiNTcFuIRC/hNBe3z/wNSOM9tv1/vewAAAABJRU5ErkJggg==";

        public static Sprite Value
        {
            get
            {
                Texture2D texture = new Texture2D (1, 1);
                texture.LoadImage(Convert.FromBase64String(Raw));
                texture.Apply();

                return Sprite.Create(
                    texture, 
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f
                );
            }
        }
    }
}