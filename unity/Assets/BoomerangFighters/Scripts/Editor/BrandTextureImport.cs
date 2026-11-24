// ─────────────────────────────────────────────────────────────
// IMPORTACIÓN DEL LOGO Y DE LOS DISEÑOS (spec v0.3).
// La PRIMERA vez que Unity importa un PNG de
//   Assets/BoomerangFighters/Resources/Marca/   (logo oficial)
//   Assets/BoomerangFighters/Resources/Disenos/ (playeras y gorras)
// le pone los ajustes que el juego necesita, para que nadie tenga que
// acordarse de hacerlo a mano:
//   • Non-Power of 2 = None: por defecto Unity estira las imágenes que
//     no miden potencia de 2 (el logo es de 1200 × 364) y saldría
//     deformado en el menú, en los muros y en el espectacular.
//   • Logo: "Alpha Is Transparency" y sin repetirse en las orillas.
//   • Diseños: Wrap Mode = Repeat (los patrones envuelven el cuerpo).
// Solo actúa cuando el archivo todavía no tiene .meta (primera
// importación): después puedes cambiar los ajustes en el Inspector y
// se respetan. Si ya tenías el logo importado de antes, pon tú
// "Non-Power of 2 = None" en su Inspector (o borra su .meta).
// No hace falta "Read/Write": el juego lee los píxeles con un
// RenderTexture (BrandArt.ReadPixels).
// ─────────────────────────────────────────────────────────────
using UnityEditor;
using UnityEngine;

namespace BoomerangFighters.EditorTools
{
    public sealed class BrandTextureImport : AssetPostprocessor
    {
        const string BrandFolder = "/BoomerangFighters/Resources/Marca/";
        const string DesignsFolder = "/BoomerangFighters/Resources/Disenos/";

        void OnPreprocessTexture()
        {
            string path = assetPath.Replace('\\', '/');
            bool brand = path.Contains(BrandFolder), design = path.Contains(DesignsFolder);
            if (!brand && !design) return;
            if (!assetImporter.importSettingsMissing) return; // ya tiene .meta: se respeta lo que haya elegido el equipo
            var importer = (TextureImporter)assetImporter;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.mipmapEnabled = true;
            if (brand)
            {
                importer.alphaIsTransparency = true;
                importer.wrapMode = TextureWrapMode.Clamp;
            }
            else importer.wrapMode = TextureWrapMode.Repeat;
        }
    }
}
