﻿using UnityEngine;
using Unity.Mathematics;

namespace kmty.geom.d3.delauney {
    using f3 = float3;
    using DN = DelaunayGraphNode3D;
    using VG = VoronoiGraph3D;
    public class Drawer3D : MonoBehaviour {
        public Material mat;
        public Material mat2;
        public int numPoint;
        public bool createMesh;
        public bool showDelaunay;
        public bool showVoronoi;
        [Range(0, 100)] public int debugDNodeId;
        [Range(0, 100)] public int debugVNodeId;

        BistellarFlip3D bf;
        VG voronoi;
        DN[] nodes;

        void Start () {
            bf = new BistellarFlip3D(numPoint, 1);
            nodes = bf.Nodes.ToArray();
            voronoi = new VG(nodes);
            if (createMesh) {
                int count = 0;
                foreach (var n in voronoi.nodes) {
                    if (true || count == debugVNodeId) {
                        var m = n.Value.Meshilify();
                        if (m != null) {
                            var g = new GameObject(count.ToString());
                            var f = g.AddComponent<MeshFilter>();
                            var r = g.AddComponent<MeshRenderer>();
                            var c = g.AddComponent<MeshCollider>();
                            r.sharedMaterial = mat2;
                            f.mesh = m;
                            c.sharedMesh = m;
                            g.transform.position = (float3)n.Value.center * 1.1f;
                            g.transform.SetParent(this.transform);
                        }
                    }
                    count++;
                }
            }
        }

        void OnRenderObject() {
            if (showDelaunay)  DrawDelauney(nodes);
            if (showVoronoi)   DrawVoronoi();
        }

        void DrawDelauney(DelaunayGraphNode3D[] nodes) {
            GL.PushMatrix();
            for (var i = 0; i < nodes.Length; i++) {
                if (i == debugDNodeId) {
                    var n = nodes[i];
                    DrawTetrahedra(n.tetrahedra, 0);
                }
            }
            GL.PopMatrix();
        }

        void DrawVoronoi() {
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            for (var i = 0; i < voronoi.nodes.Count; i++) {
                if (i == debugVNodeId) {
                    var n = voronoi.nodes[i];
                    foreach (var s in n.segments) {
                        mat.SetPass(3);
                        GL.Vertex((f3)s.sg.a);
                        GL.Vertex((f3)s.sg.b);
                    }
                }
            }
            GL.End();
            GL.PopMatrix();
        }

        void DrawTetrahedra(Tetrahedra t, int pass) {
            mat.SetPass(pass);
            GL.Begin(GL.LINE_STRIP); GL.Vertex((f3)t.a); GL.Vertex((f3)t.b); GL.Vertex((f3)t.c); GL.End();
            GL.Begin(GL.LINE_STRIP); GL.Vertex((f3)t.a); GL.Vertex((f3)t.c); GL.Vertex((f3)t.d); GL.End();
            GL.Begin(GL.LINE_STRIP); GL.Vertex((f3)t.a); GL.Vertex((f3)t.d); GL.Vertex((f3)t.b); GL.End();
        }
    }
}
