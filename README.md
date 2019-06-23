# TerrainBuilder
it builds terrain
Am besten auf bearbeiten drücken, keine Ahnung wieso Git das Output nicht formatiert......

Dies ist der Terraingenerierer von:
Henry Miller,
Sina Müller,
Marcel Sauter,
Julien Jakob,

Unity Version:
2019.1.3f1

Zu startende Szene:
SampleScene

Folgend befinden sich die Lösungen und die, für die Lösung der Aufgabe relevanten, Assets sowie branches. 
Zudem befindet sich bei Aufgabe 5 eine kurze Erklärung zu den Extras.
Für die genauere Funktionsweise und Möglichkeiten siehe Kommentare im Code.

Zu den Lösungen der Aufgaben:
Aufgabe 1:
Siehe branch "Aufgabe-1"
Lösung siehe 
Skripte:
  DiamondSquareAlgorithm
  MeshSpecs
  TerrainBuilder
  
Aufgabe 2:
Siehe Branch "Aufgabe-2"
Lösung siehe 
Skript:
  TerrainManipulator
  
Aufgabe 3:
Siehe Branch "Aufgabe-3"
Lösung siehe 
Skript:
  ContourLines
Shader:
  ColorAndWaterShader
Texturen:
  dayColorMap
  
Aufgabe 4:
Siehe Branch "Aufgabe-4"
Lösung siehe 
Skript:
  WaterMovementScript
Shader:
  ColorAndWaterShader
Texturen:
  WaterTexture
  WaterNormalMap1
  WaterNormalMap2
  
Aufgabe 5:
Siehe Branch "Aufgabe-5" und Andere
Lösung siehe Skript:
  NightMode           <- Ändert das Aussehen des Terrains von Wasser- zu Lavalandschaft => nutzung über Boolean im Inspector.
                         Nutzt Texturen: nightColorMap und LavaTexture
  CameraMovement      <- Bewegt die Camera frei auf allen Achsen => siehe beim an Main Camera angebrachten Skript im Inspector.
  TerrainManipulator  <- Allgemein führt Mausradzurück oder rechte Maustaste zum verringern der Höhe und linke Maustaste sowie                                    Mausradvor zum erhöhen der Höhe  
                         Siehe setZero => setzt alle Höhen im Radius auf 0;
                         Siehe createCrater => erzeugt ein kraterähnliches Gebilde im gegebenen Radius
                         Siehe changeVertexContinuously => ermöglicht es kontinuierlich die Tools zu benutzen oder, wenn false, die                                                                 Manipulationen mit dem Mausrad durchzuführen
                         Siehe addRandomOffset  => manipuliert die Höhenänderung an jedem Vertex durch Zufallswerte, welche eingestellt                                                    werden können
  ColorHeightMap      <- Leider nur ein angefangener Versuch das Terrain über eine heightMap zu erstellen sowie zu manipulieren, was das                          ständige Durchsuchen der Vertices erspart und damit den Terrainbuilder performanter gemacht hätte
                          
