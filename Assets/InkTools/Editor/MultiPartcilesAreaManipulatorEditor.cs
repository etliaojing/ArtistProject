using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MultiParticlesAreaManipulator))]
public class MultiParticlesAreaManipulatorEditor : Editor
{
    bool m_debug = false;
    int m_selected = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MultiParticlesAreaManipulator emitter = (MultiParticlesAreaManipulator)target;

        if (emitter.m_particlesAreaObject == null)
        {
            EditorGUILayout.HelpBox("Fluid particles not defined", MessageType.Error);
        }
        else //if(emitter.m_particlesArea == null)
        {
            
            MultiParticlesArea[] targets = emitter.m_particlesAreaObject.GetComponents<MultiParticlesArea>();
            string[] options = new string[targets.Length];

            for(int i = 0; i < targets.Length; ++i)
            {
                options[i] = targets[i].ToString() + i.ToString();
                if (emitter.m_particlesArea != null && emitter.m_particlesArea == targets[i])
                    m_selected = i;
            }

            m_selected = EditorGUILayout.Popup(m_selected, options);
            emitter.m_particlesArea = targets[m_selected];
        }

        // Strength
        emitter.m_strength = EditorGUILayout.Slider("Strength", emitter.m_strength / 1000f, 0.0f, 10.0f) * 1000f;

        // Scale
        emitter.m_useScaleAsSize = !EditorGUILayout.Toggle("Set Size Manually", !emitter.m_useScaleAsSize);
        if (emitter.m_useScaleAsSize)
        {
            EditorGUILayout.HelpBox(" Using global scale as size", MessageType.None);
        }
        else
        {
            ++EditorGUI.indentLevel;
            emitter.m_radius = EditorGUILayout.Slider("Radius", emitter.m_radius, 0.0f, 5.0f);
            --EditorGUI.indentLevel;
        }

        // Debug
        m_debug = EditorGUILayout.Foldout(m_debug, "Debug");
        if (m_debug)
        {
            ++EditorGUI.indentLevel;
            emitter.m_showGizmo = EditorGUILayout.Toggle("Draw Gizmo", emitter.m_showGizmo);
            --EditorGUI.indentLevel;
        }

        EditorUtility.SetDirty(emitter);
    }
}
