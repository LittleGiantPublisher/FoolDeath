using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
[UnityEditor.InitializeOnLoad]
public class AfilStartWindow : EditorWindow
{
    public static bool opened;
    
    private bool pkgs,achd;

    [MenuItem("Afil/Porting/Show Start Window")]
    public static void ShowAfilStart()
    {
        AfilStartWindow wnd = GetWindow<AfilStartWindow>();
        wnd.titleContent = new GUIContent("MyEditorWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // VisualElements objects can contain other VisualElement following a tree hierarchy

       
 
     
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        Texture2D LogoTex = (Texture2D)Resources.Load("EdlogoAfil"); //don't put png
        GUILayout.Label(LogoTex, GUILayout.MaxWidth(100f), GUILayout.MaxHeight(100f));
        GUI.color = Color.yellow;
        GUI.skin.label.fontSize = 30;
        GUILayout.Label("Bem vindo ao sistema de Porting da Afil");
        GUILayout.EndVertical();
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 28;
        GUI.color = Color.white;
        GUILayout.Label("Algumas Informa��es:");
        GUI.skin.label.fontSize = 20;
        GUILayout.TextField("Para cada plataforma existem packages que devem ser instalados, consulte a lista logo abaixo:\nPara que os achievements funcionem, voc� deve configurar o \nAchievementData na pasta Resources, por favor n�o renomeie e nem mude ele de local.\n A primeira cena deve ser 'AfilLogo', a segunda 'LoadControlleManager' e a terceira a cena que inicia o jogo\n As cenas est�o em 'Assets/PortingScenes'");

        if(GUILayout.Button("Mostrar depend�ncias de Packages"))
        {
            pkgs = !pkgs;
        }
        if (pkgs)
        {
            GUILayout.Label("As vers�es podem ser diferentes:");
            Texture2D ipkgs = (Texture2D)Resources.Load("EdPakages"); //don't put png
            GUILayout.Label(ipkgs, GUILayout.MaxWidth(400f), GUILayout.MaxHeight(400f));
        }
        if (GUILayout.Button("Baixar depend�ncias de Packages"))
        {
            AfilPortingEditor.Download();
        }
        

        if (GUILayout.Button("Abrir AchievementData"))
        {
            AfilPortingEditor.CheckAchievementsData();
            var data = Resources.Load<AchievementData>("AchievementData");
            if(data == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("Faltando data", "AchievementData n�o foi encontrado", "OK");
                return;
            }
            AssetDatabase.OpenAsset(data);
        }
        GUI.skin.label.fontSize = 28;
        GUI.color = Color.red;
        GUILayout.Label("N�o esque�a de que toda a modifica��o do package deve ser feito\n no git pr�prio dele e o package deve ser reimportado\n na master e posteriormente feito merge para as \nbranchs das plataformas.");
        GUI.color = Color.green;
        if (GUILayout.Button("Fechar e n�o mostrar mais no in�cio"))
        {
            Close();
            EditorPrefs.SetBool("NAfilStartWindow", true);

        }
    }
}
[UnityEditor.InitializeOnLoad]
public class NAfilPortingEditor: AssetPostprocessor
{


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload) {
        if (EditorPrefs.GetBool("NAfilStartWindow") == true || AfilStartWindow.opened)
            return;
        AfilStartWindow.ShowAfilStart();
        AfilStartWindow.opened = true;
        
    }

}