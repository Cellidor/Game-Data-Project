using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using UnityEditor;
using System.Xml;

public class LevelManager : MonoBehaviour {

    public Transform groundHolder;
    public Transform turretHolder;

    //enum is used to more specifically define given values used further down. This way, when referencing "MapDataFormat", it's 
    //easier to understand what "Base64" and "CSV" means than using a variable you set to an int of "0" or "1".
    public enum MapDataFormat {
        Base64,
        CSV
    }
    //Reference variable has been set for MapDataFormat.
    public MapDataFormat mapDataFormat;
    //a Sprite Sheet will be neededto represent the map we wish to load.
    public Texture2D spriteSheetTexture;

    //A given tile to be placed.
    public GameObject tilePrefab;
    //A given turret to be placed.
    public GameObject turretPrefab;
    //A list made for all map sprites used for a given map.
    public List<Sprite> mapSprites;
    //A list of all "tile" game objects used in the scene.
    public List<GameObject> tiles;
    //Each placed turret will nee a vector3 reference for their location.
    public List<Vector3> turretPositions;

    //Offset used to change center of the tile.
    Vector3 tileCenterOffset;
    //Offset used to change the center of the map itself.
    Vector3 mapCenterOffset;

    //the name of a needed TMX File a user can input.
    public string TMXFilename;

    //The location of the game.
    string gameDirectory;
    //The location of the game's data.
    string dataDirectory;
    //The location of the game's map.
    string mapsDirectory;
    //The location of the sprite sheet used for the game.
    string spriteSheetFile;
    //The name of the TMX file.
    string TMXFile;

    //How many pixels each unit takes up.
    public int pixelsPerUnit = 32;

    //The width of a given tile in pixels.
    public int tileWidthInPixels;
    //The height of a given tile in pixels.
    public int tileHeightInPixels;
    //The width of a given tile.
    float tileWidth;
    //The height of a given tile.
    float tileHeight;

    //How many colums the sprite sheet has.
    public int spriteSheetColumns;
    //How many rows the sprite sheet has.
    public int spriteSheetRows;

    //How many columns the map contains.
    public int mapColumns;
    //How many rows the map contains.
    public int mapRows;

    //The name of the map data in the ofrm of a string.
    public string mapDataString;
    //the data fo a given map laid out in a list of int values.
    public List<int> mapData;




    // from http://answers.unity3d.com/questions/10580/editor-script-how-to-clear-the-console-output-wind.html
    static void ClearEditorConsole() {
        // The method below obtains the currently open scene view, obtains the log entries from the console, and clears them, much like the 
        // "Clear on play" button does, but within the editor. 
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type type = assembly.GetType("UnityEditorInternal.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    static string GetJoinedUnixPath(string partA, string partB) {
        // The below function takes in two parts of a file path and merges them together, but changes
        // each instance of "\\" with "/" so it will work with the proper syntax.
        return Path.GetFullPath(Path.Combine(partA, partB)).Replace("\\", "/");
    }

    static void DestroyChildren(Transform parent) {
        // A simple function that, when given a parent, will destroy all children within a given parent transform.
        // "DestroyImmediate" is used here as the standard "Destroy" doesn't work in editor mode as no frames are running.
        for (int i = parent.childCount - 1; i >= 0; i--) {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }



    // NOTE: CURRENTLY ONLY WORKS WITH A SINGLE TILED TILESET

    public void LoadLevel() {

        // Clear both editor console and all children in "ground" and "turret" holder parents.
        ClearEditorConsole();
        DestroyChildren(groundHolder);
        DestroyChildren(turretHolder);

        // Thanks to the script below, rather than having to find the exact location of the TMX file by hand
        // each time it needs to be used, the script below will instead automatically go into the game's data folder
        // through "Data/Maps/" and return the directory location of the TMX file we need using only the TMX filename.
        {
            gameDirectory = Environment.CurrentDirectory;
            dataDirectory = GetJoinedUnixPath(Directory.GetParent(gameDirectory).FullName, "Data");
            mapsDirectory = GetJoinedUnixPath(dataDirectory, "Maps");
            TMXFile = GetJoinedUnixPath(mapsDirectory, TMXFilename);
        }


        // The script below is, overall, used to parse the XML data of our TMX file for use within Unity.
        {
            //Clear any current map data
            mapData.Clear();

            //Create a string and fill it with all text contained within our TMX file.
            string content = File.ReadAllText(TMXFile);

            // XMLreader is used to turn the given string information from an XML file and translate it into
            // a format where a user can actually read and understand whats present.
            using (XmlReader reader = XmlReader.Create(new StringReader(content))) {

                // Read through the content, and translate the mapColumns and MapRows into Ints to be set underneath the
                // "width" and "height" for the map.
                reader.ReadToFollowing("map");
                mapColumns = Convert.ToInt32(reader.GetAttribute("width"));
                mapRows = Convert.ToInt32(reader.GetAttribute("height"));

                // Perform the same operation as above, only creating an int value for tileset giving the height in pixels
                // for the attributes "tilewidth" and "tileheight"
                reader.ReadToFollowing("tileset");
                tileWidthInPixels = Convert.ToInt32(reader.GetAttribute("tilewidth"));
                tileHeightInPixels = Convert.ToInt32(reader.GetAttribute("tileheight"));

                // Grab an int of the overall tile count of the sheet, and the number of tiles in the column.
                // Rather than doing the same for the rows, one can simply divide the overall tile count by the columns
                // and get the number of rows as the result.
                int spriteSheetTileCount = Convert.ToInt32(reader.GetAttribute("tilecount"));
                spriteSheetColumns = Convert.ToInt32(reader.GetAttribute("columns"));
                spriteSheetRows = spriteSheetTileCount / spriteSheetColumns;

                // Read until "image" has been hit, then return the full file path of the sprite sheet file.
                reader.ReadToFollowing("image");
                spriteSheetFile = GetJoinedUnixPath(mapsDirectory, reader.GetAttribute("source"));

                // Read up to "layer". This will be used later for multiple layers.
                reader.ReadToFollowing("layer");

                // Read to "data" and "encoding" to set "mapDataFormat" to base64 or csv depending on what the xml reads as.
                reader.ReadToFollowing("data");
                string encodingType = reader.GetAttribute("encoding");

                switch (encodingType) {
                    case "base64":
                        mapDataFormat = MapDataFormat.Base64;
                        break;
                    case "csv":
                        mapDataFormat = MapDataFormat.CSV;
                        break;
                }

                // trims away any leading and trailing whitespace from our "reader" string value.
                mapDataString = reader.ReadElementContentAsString().Trim();

                //Reset all turrent positions currently in place.
                turretPositions.Clear();

                // If the reader contains an "objectgroup" setting, then, so long as there are objects to read,
                // set their x and y turrent positions based on the attribute as found in the XML file.
                if (reader.ReadToFollowing("objectgroup")) {
                    if (reader.ReadToDescendant("object")) {
                        do {
                            float x = Convert.ToSingle(reader.GetAttribute("x")) / (float)pixelsPerUnit;
                            float y = Convert.ToSingle(reader.GetAttribute("y")) / (float)pixelsPerUnit;
                            turretPositions.Add(new Vector3(x, -y, 0));

                        } while (reader.ReadToNextSibling("object"));
                    }
                }

            }

            // Parse the xml data differently based on what format the file has been made in
            switch (mapDataFormat) {

                // in Base64, the information is represented in 64 bits, and as such must be converted from that format into
                // a readable string.
                case MapDataFormat.Base64:

                    byte[] bytes = Convert.FromBase64String(mapDataString);
                    int index = 0;
                    while (index < bytes.Length) {
                        int tileID = BitConverter.ToInt32(bytes, index) - 1;
                        mapData.Add(tileID);
                        index += 4;
                    }
                    break;


                case MapDataFormat.CSV:
                    // in CSV, or "Comma Separated Values", every piece of information is split using quotations and commas.
                    // The below methods account for this and put the information into a readoable format from this.
                    string[] lines = mapDataString.Split(new string[] { " " }, StringSplitOptions.None);
                    foreach (string line in lines) {
                        string[] values = line.Split(new string[] { "," }, StringSplitOptions.None);
                        foreach (string value in values) {
                            int tileID = Convert.ToInt32(value) - 1;
                            mapData.Add(tileID);
                        }
                    }
                    break;

            }

        }


        {
            // Simple math is used to obtain tile width and height from the overall pixels per unit and the width/height in pixels.
            tileWidth = (tileWidthInPixels / (float)pixelsPerUnit);
            tileHeight = (tileHeightInPixels / (float)pixelsPerUnit);

            //take half ot the tile width and height to find the center of each, similarily to find center offset.
            tileCenterOffset = new Vector3(.5f * tileWidth, -.5f * tileHeight, 0);
            mapCenterOffset = new Vector3(-(mapColumns * tileWidth) * .5f, (mapRows * tileHeight) * .5f, 0);

        }




        // Create a new 2D texture, size 2 by 2, loading the image in "spriteSheetFile" and settign the texture's 
        // filter mode to point and wrap mode to clamp. Point keeps the pixels looking crisp (though blocky up close), and
        // Clamp can help reduce artifacts foun on the edges of the texture.
        {
            spriteSheetTexture = new Texture2D(2, 2);
            spriteSheetTexture.LoadImage(File.ReadAllBytes(spriteSheetFile));
            spriteSheetTexture.filterMode = FilterMode.Point;
            spriteSheetTexture.wrapMode = TextureWrapMode.Clamp;
        }


        // Clear current sprite list, then, using the sprite sheet, create a new sprite for each and every tile within the sheet. 
        // The definition of each tile is based on createed rectangles using current location on the sprite sheet, the height/width  
        // of each pixel, and a pivot in the middle of each sprite. This repeats until each part of hte sprite sheet has been accounted for.
        {
            mapSprites.Clear();

            for (int y = spriteSheetRows - 1; y >= 0; y--) {
                for (int x = 0; x < spriteSheetColumns; x++) {
                    Sprite newSprite = Sprite.Create(spriteSheetTexture, new Rect(x * tileWidthInPixels, y * tileHeightInPixels, tileWidthInPixels, tileHeightInPixels), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                    mapSprites.Add(newSprite);
                }
            }
        }

        // Clear current tile list. Then perform the following; take the current x and y int value as the location of the sprite to be loaded,
        // and make an int "tileID" based on the tile ID from our map data. Then, instantiate a new tile at the given location, taking into
        // account the offset of the map. Change the sprite of the instantiated object to one that matches the tile data from our XML file.
        // Finally, make the instantiate sprite a child of he "groundHolder" script, and add that game object to the "tiles" list.
        {
            tiles.Clear();

            for (int y = 0; y < mapRows; y++) {
                for (int x = 0; x < mapColumns; x++) {

                    int mapDatatIndex = x + (y * mapColumns);
                    int tileID = mapData[mapDatatIndex];

                    GameObject tile = Instantiate(tilePrefab, new Vector3(x * tileWidth, -y * tileHeight, 0) + mapCenterOffset + tileCenterOffset, Quaternion.identity) as GameObject;
                    tile.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = mapSprites[tileID];
                    tile.transform.parent = groundHolder;
                    tiles.Add(tile);
                }
            }
        }


        // For every given turret position in our turret positions list, instantiage a turret prefab at that location, name
        // the game object "turret" and make it a child of the "turretHolder" game object.
        {
            foreach (Vector3 turretPosition in turretPositions) {
                GameObject turret = Instantiate(turretPrefab, turretPosition + mapCenterOffset, Quaternion.identity) as GameObject;
                turret.name = "Turret";
                turret.transform.parent = turretHolder;
            }
        }

        // Obtain the current time, then print it into the console to determine at what time the level was loaded.
        DateTime localDate = DateTime.Now;
        print("Level loaded at: " + localDate.Hour + ":" + localDate.Minute + ":" + localDate.Second);
    }
}


