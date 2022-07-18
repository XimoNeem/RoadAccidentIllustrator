using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace RoadAccidentIllustrator
{
    namespace RAI_UI
    {
        public enum ToolbarItemType
        {
            MainPage,
            Account,
            AddObject,
            ObjectSettings,
            VisualizationSettings,
            RenderSettings,
            SavePage,
            MainSettings,
            TipsPage,
            DevStuff
        }

        [System.Serializable]
        public class ToolbarItem
        {
            public List<GameObject> items;
            public ToolbarItemType type;
        }
    }

    namespace RAI_Events
    {
        [System.Serializable]
        public class RAI_EventItem
        {
            public RAI_EventType eventType;
            public UnityEvent eventItem;
        }

        public enum RAI_EventType
        {
            OnObjectMove,
            OnObjectRotate,
            OnObjectInfo,
            OnObjectSelected,
            OnObjectDeselected,
            OnObjectCreated,
            OnMenuItemSelected
        }
    }

    namespace RAI_Mover
    {
        public enum MoveItemType
        {
            Move,
            Rotate,
            Info
        }
    }

    namespace RAI_ObjectSettings
    {
        #region objectTypes
        public enum RAI_ObjectType
        {
            Road,
            Vehicle,
            TrafficSign,
            TrafficLight,
            RoadMark,
            Decoration
        }

        public enum RoadType
        {
            Дорожное_полотно,
            Перекресток
        }

        public enum VehicleType
        {
            Легковой_автомобиль,
            Грузовик,
            Мотоцикл,
            Велосипед
        }

        public enum DecorationType
        {
            Постройка,
            Персонаж,
            Растительность,
            Элементы_ОДД
        }

        public enum RoadMarkType
        {
            Принт,
            Линейная_разметка
        }

        public enum TrafficLightType
        {
            Фонарь,
            Светофор
        }

        public enum TrafficSignType
        {
            Дорожный_знак,
            Объект_на_дороге
        }

        #endregion objectTypes

        #region objectSettings

        public enum trafficLightSettingType
        {
            tl_3x1,
            tl_2x1,
            tl_1x1,
            tl_3xl2,
            tl_3xr2,
            tl_3x3,
            tl_1x3,
        }
        public enum trafficLightSignalType
        {
            none,
            green,
            yellow,
            red
        }
        public enum trafficLightPosition
        {
            up,
            center,
            down,
            left,
            right
        }

        public enum roadWidth
        {
            Одна_полоса,
            Две_полосы,
            Три_полосы,
            Четыре_полосы,
            Пять_полос,
            Шесть_полос,
            Восемь_полос,
            Десять_полос
        }

        public enum roadPointTypes
        {
            Линия,
            Сплошная_линий,
            Прерывистая_линия,
            Двойная_сплошная_линия,
            линия2
        }

        public enum settingType  //тип объека, который нужно настроить
        {
            none = 100,
            vehicle_regular = 101,
            vehiclr_special = 102,

            roadsign = 201,

            trafficlight_3x1 = 301,
            trafficlight_2x1 = 302,
            trafficlight_3xl2 = 303,
            trafficlight_3xr2 = 304,
            trafficlight_3x3 = 305,
            trafficlight_1x3 = 306,
            trafficlight_1x1 = 307,

            road_crossroad = 401,
            road_roadpoint = 402,

            roadprint_point = 501,
            roadprint_print = 502,

            road_crossroadCenter = 601,
            road_crossroadPoint = 602
        }
        public enum settingItemType  //тип настройки (параметр)
        {
            vehicle_leftturn,
            vehicle_rightturn,
            vehicle_parkinglights,
            vehicle_stoplights,
            vehicle_lights,
            vehicle_farlights,
            vehicle_color,
            vehicle_speciallight,

            roadsign_sign,

            traffic_light,

            road_border,
            road_dustborder,
            road_bumpborder,
            road_bumpborder_center,
            road_width,

            roadmark_roadmarkline,
            roadmark_roadprint,
            roadmark_roadmarklinewidth,
            roadmark_roadmarklinecolor,
        }
        public enum containerType
        {
            BoolContainer,
            FloatContainer,
            EnumContainer,
            ColorContainer,
            RoadSignContainer,
            TrafficLightContainer
        }
        [System.Serializable]
        public struct DefaultValue
        {
            public settingItemType type;
            public bool BoolValue;
            public float FloatValue;
            public int IntValue;
        }

        public class TrafficLightSetting
        {
            public trafficLightSignalType lightType;
            public trafficLightPosition lightPosition;

            public TrafficLightSetting(trafficLightPosition pos)
            {
                lightPosition = pos;
                lightType = trafficLightSignalType.none;
            }
        }

        [System.Serializable]
        public class SettingContainer  //От чего наследуемся
        {
            public string name;
            public settingItemType type;
            public virtual SettingContainer GetValue()
            {
                return this;
            }
        }
        [System.Serializable]
        public class BoolContainer : SettingContainer
        {
            public bool value;

            public override SettingContainer GetValue()
            {
                return this;
            }

            public BoolContainer(string name, bool newValue, settingItemType type)
            {
                base.name = name;
                base.type = type;
                value = newValue;
            }
        }
        [System.Serializable]
        public class RoadSignContainer : SettingContainer
        {
            public Texture2D texture;
            public string textureName;

            public override SettingContainer GetValue()
            {
                return this;
            }

            public RoadSignContainer(string name, settingItemType type)
            {
                base.name = name;
                base.type = type;
            }
        }
        [System.Serializable]
        public class TextureContainer : SettingContainer
        {
            public Texture2D texture;
            public string textureName;

            public override SettingContainer GetValue()
            {
                return this;
            }

            public TextureContainer(string name, settingItemType type, Texture2D tex)
            {
                base.name = name;
                base.type = type;
                texture = tex;
                textureName = tex.name;
            }
        }
        [System.Serializable]
        public class TrafficLightContainer : SettingContainer
        {
            public trafficLightSettingType trafficLightType;
            public List<TrafficLightSetting> textures;

            public override SettingContainer GetValue()
            {
                return this;
            }

            public TrafficLightContainer(string name, settingItemType type)
            {
                base.name = name;
                base.type = type;
            }
        }
        [System.Serializable]
        public class FloatContainer : SettingContainer
        {
            public float value;

            public float minValue;
            public float maxValue;

            public override SettingContainer GetValue()
            {
                return this;
            }

            public FloatContainer(string name, settingItemType type, float newValue, float min, float max)
            {
                base.name = name;
                base.type = type;
                value = newValue;
                minValue = min;
                maxValue = max;
            }
        }
        [System.Serializable]
        public class ColorContainer : SettingContainer
        {
            public Color value = Color.white;

            public override SettingContainer GetValue()
            {
                return this;
            }

            public ColorContainer(string name, Color newValue, settingItemType type)
            {
                base.name = name;
                base.type = type;
                value = newValue;
            }
        }
        [System.Serializable]
        public class EnumContainer : SettingContainer
        {
            public string[] names;
            public int value;

            public override SettingContainer GetValue()
            {
                return this;
            }

            public EnumContainer(string name, settingItemType type, string[] valueNames, int nameValue)
            {
                base.name = name;
                base.type = type;

                names = valueNames;
                value = nameValue;
            }
        }

        #endregion objectSettings

        #region SettingsClasses
        [System.Serializable]
        public class VehicleSettings
        {
            public bool leftTurn, rightTurn, stopLight, parkingLight, light, farLight, specialLight;
        }
        [System.Serializable]
        public class RoadMarkPointSettings
        {
            public int textureIndex;
            public float width;
            public Color color;
        }

        #endregion SettingsClasses

        #region UI

        [System.Serializable]
        public class SettingUIItem
        {
            public RAI_ObjectType type;
            public List<GameObject> items;
        }

        #endregion UI

        public static class SettingsManager
        {
            public static List<SettingContainer> GetSettingsByType(settingType type)  //  Задаем настройки
            {
                List<SettingContainer> result = new List<SettingContainer>();

                if (type == settingType.vehicle_regular)
                {
                    result.Add(new BoolContainer("Правые поворотники", false, settingItemType.vehicle_rightturn));
                    result.Add(new BoolContainer("Левые  поворотники", false, settingItemType.vehicle_leftturn));
                    result.Add(new BoolContainer("Габариты", true, settingItemType.vehicle_parkinglights));
                    result.Add(new BoolContainer("Стоп-сигналы", false, settingItemType.vehicle_stoplights));
                    result.Add(new BoolContainer("Ближний свет", false, settingItemType.vehicle_lights));
                    result.Add(new BoolContainer("Дальний свет", false, settingItemType.vehicle_farlights));
                    result.Add(new ColorContainer("Цвет корпуса", Color.white, settingItemType.vehicle_color));
                }

                else if (type == settingType.vehiclr_special)
                {
                    result.Add(new BoolContainer("Специальные сигналы", false, settingItemType.vehicle_speciallight));
                    result.Add(new BoolContainer("Правые поворотники", false, settingItemType.vehicle_rightturn));
                    result.Add(new BoolContainer("Левые  поворотники", false, settingItemType.vehicle_leftturn));
                    result.Add(new BoolContainer("Габариты", true, settingItemType.vehicle_parkinglights));
                    result.Add(new BoolContainer("Стоп-сигналы", false, settingItemType.vehicle_stoplights));
                    result.Add(new BoolContainer("Ближний свет", false, settingItemType.vehicle_lights));
                    result.Add(new BoolContainer("Дальний свет", false, settingItemType.vehicle_farlights));
                }

                else if (type == settingType.roadsign)
                {
                    result.Add(new RoadSignContainer("Тип знака", settingItemType.roadsign_sign));
                }

                else if (type == settingType.trafficlight_3x1)
                {
                    TrafficLightContainer cont = new TrafficLightContainer("Сигналы светофора", settingItemType.traffic_light);

                    cont.trafficLightType = trafficLightSettingType.tl_3x1;

                    cont.textures = new List<TrafficLightSetting>();

                    TrafficLightSetting tls = new TrafficLightSetting(trafficLightPosition.up);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.center);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.down);
                    cont.textures.Add(tls);

                    result.Add(cont);
                }
                else if (type == settingType.trafficlight_2x1)
                {
                    TrafficLightContainer cont = new TrafficLightContainer("Сигналы светофора", settingItemType.traffic_light);

                    cont.trafficLightType = trafficLightSettingType.tl_2x1;

                    cont.textures = new List<TrafficLightSetting>();

                    TrafficLightSetting tls = new TrafficLightSetting(trafficLightPosition.up);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.center);
                    cont.textures.Add(tls);

                    result.Add(cont);
                }
                else if (type == settingType.trafficlight_1x1)
                {
                    TrafficLightContainer cont = new TrafficLightContainer("Сигнал светофора", settingItemType.traffic_light);

                    cont.trafficLightType = trafficLightSettingType.tl_1x1;

                    cont.textures = new List<TrafficLightSetting>();

                    TrafficLightSetting tls = new TrafficLightSetting(trafficLightPosition.center);
                    cont.textures.Add(tls);

                    result.Add(cont);
                }
                else if (type == settingType.trafficlight_3x3)
                {
                    TrafficLightContainer cont = new TrafficLightContainer("Сигналы светофора", settingItemType.traffic_light);

                    cont.trafficLightType = trafficLightSettingType.tl_3x3;

                    cont.textures = new List<TrafficLightSetting>();

                    TrafficLightSetting tls = new TrafficLightSetting(trafficLightPosition.up);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.center);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.down);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.left);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.right);
                    cont.textures.Add(tls);

                    result.Add(cont);
                }
                else if (type == settingType.trafficlight_3xl2)
                {
                    TrafficLightContainer cont = new TrafficLightContainer("Сигналы светофора", settingItemType.traffic_light);
                    cont.trafficLightType = trafficLightSettingType.tl_3xl2;
                    cont.textures = new List<TrafficLightSetting>();
                    TrafficLightSetting tls = new TrafficLightSetting(trafficLightPosition.up);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.center);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.down);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.left);
                    cont.textures.Add(tls);
                    result.Add(cont);
                }
                else if (type == settingType.trafficlight_3xr2)
                {
                    TrafficLightContainer cont = new TrafficLightContainer("Сигналы светофора", settingItemType.traffic_light);
                    cont.trafficLightType = trafficLightSettingType.tl_3xr2;
                    cont.textures = new List<TrafficLightSetting>();
                    TrafficLightSetting tls = new TrafficLightSetting(trafficLightPosition.up);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.center);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.down);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.right);
                    cont.textures.Add(tls);
                    result.Add(cont);
                }
                else if (type == settingType.trafficlight_1x3)
                {
                    TrafficLightContainer cont = new TrafficLightContainer("Сигналы светофора", settingItemType.traffic_light);
                    cont.trafficLightType = trafficLightSettingType.tl_1x3;
                    cont.textures = new List<TrafficLightSetting>();
                    TrafficLightSetting tls = new TrafficLightSetting(trafficLightPosition.up);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.center);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.left);
                    cont.textures.Add(tls);
                    tls = new TrafficLightSetting(trafficLightPosition.right);
                    cont.textures.Add(tls);
                    result.Add(cont);
                }

                else if (type == settingType.roadprint_point)
                {
                    roadPointTypes types = new roadPointTypes();
                    string[] namesList = System.Enum.GetNames(types.GetType());
                    result.Add(new EnumContainer("Тип разметки", settingItemType.roadmark_roadmarkline, namesList, 0));
                    result.Add(new FloatContainer("Ширина", settingItemType.roadmark_roadmarklinewidth, 1, 1, 4));
                    result.Add(new ColorContainer("Цвет", Color.white, settingItemType.roadmark_roadmarklinecolor));
                }
                else if (type == settingType.roadprint_print)
                {
                    roadPointTypes types = new roadPointTypes();
                    string[] namesList = System.Enum.GetNames(types.GetType());
                    result.Add(new TextureContainer("Принт", settingItemType.roadmark_roadprint, RoadPrintListController.FindObjectOfType<RoadPrintListController>().group_1[0]));
                }

                else if (type == settingType.road_roadpoint)
                {
                    roadWidth types = new roadWidth();
                    string[] namesList = System.Enum.GetNames(types.GetType());
                    for (int i = 0; i < namesList.Length; i++)
                    {
                        namesList[i] = namesList[i].Replace("_", " ");
                    }
                    result.Add(new EnumContainer("Полосы", settingItemType.road_width, namesList, 0));
                    result.Add(new BoolContainer("Обочина", false, settingItemType.road_dustborder));
                    result.Add(new BoolContainer("Отбойник", false, settingItemType.road_bumpborder));
                    result.Add(new BoolContainer("Бордюр", false, settingItemType.road_border));
                    result.Add(new BoolContainer("Центральный отбойник", false, settingItemType.road_bumpborder_center));
                }

                else if (type == settingType.road_crossroadPoint)
                {
                    roadWidth types = new roadWidth();
                    string[] namesList = System.Enum.GetNames(types.GetType());
                    for (int i = 0; i < namesList.Length; i++)
                    {
                        namesList[i] = namesList[i].Replace("_", " ");
                    }
                    result.Add(new EnumContainer("Полосы", settingItemType.road_width, namesList, 0));
                }

                else if (type == settingType.road_crossroadCenter)
                {
                    result.Add(new BoolContainer("Обочина", false, settingItemType.road_dustborder));
                    result.Add(new BoolContainer("Отбойник", false, settingItemType.road_bumpborder));
                    result.Add(new BoolContainer("Бордюр", false, settingItemType.road_border));
                    result.Add(new BoolContainer("Центральный отбойник", false, settingItemType.road_bumpborder_center));
                }

                return result;
            }

            public static bool GetBoolValue(Movable item, settingItemType type)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        BoolContainer container = (BoolContainer)settingItem;
                        return container.value;
                    }
                }
                throw new RAI_Exceptions.RAI_Exception.NoSettingException("no bool found!");
            }

            public static int GetIntValue(Movable item, settingItemType type)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        EnumContainer container = (EnumContainer)settingItem;
                        return container.value;
                    }
                }

                throw new RAI_Exceptions.RAI_Exception.NoSettingException("no bool found!");
            }

            public static Texture2D GetTextureValue(Movable item, settingItemType type)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        TextureContainer container = (TextureContainer)settingItem;
                        return container.texture;
                    }
                }
                throw new RAI_Exceptions.RAI_Exception.NoSettingException("no bool found!");
            }
            public static float GetFloatValue(Movable item, settingItemType type)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        FloatContainer container = (FloatContainer)settingItem;
                        return container.value;
                    }
                }
                throw new RAI_Exceptions.RAI_Exception.NoSettingException("no bool found!");
            }
            public static Texture2D GetTrafficSignValue(Movable item, settingItemType type)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        RoadSignContainer container = (RoadSignContainer)settingItem;
                        return container.texture;
                    }
                }
                throw new RAI_Exceptions.RAI_Exception.NoSettingException("no texture found!");
            }
            public static Color GetColorValue(Movable item, settingItemType type)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        ColorContainer container = (ColorContainer)settingItem;
                        return container.value;
                    }
                }

                throw new RAI_Exceptions.RAI_Exception.NoSettingException("no texture found!");
            }
            public static List<TrafficLightSetting> GetTrafficLightValues(Movable item)
            {
                TrafficLightContainer container = (TrafficLightContainer)item.objectSettings[0];
                return container.textures;
            }
            public static bool HasSetting(settingItemType type, Movable item)
            {
                bool result = false;

                foreach (var setting in item.objectSettings)
                {
                    if (setting.type == type)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
            public static SettingContainer GetSettingContainer(Movable item, settingItemType type)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        return settingItem;
                    }
                }
                throw new RAI_Exceptions.RAI_Exception.NoSettingException("no texture found!");
            }
            public static containerType GetSettingType(SettingContainer container)
            {
                if (container is BoolContainer)
                {
                    return containerType.BoolContainer;
                }
                else if (container is FloatContainer)
                {
                    return containerType.FloatContainer;
                }
                else if (container is EnumContainer)
                {
                    return containerType.EnumContainer;
                }
                else if (container is ColorContainer)
                {
                    return containerType.ColorContainer;
                }
                else if (container is RoadSignContainer)
                {
                    return containerType.RoadSignContainer;
                }
                else if (container is TrafficLightContainer)
                {
                    return containerType.TrafficLightContainer;
                }
                throw new RAI_Exceptions.RAI_Exception.NoSettingException("Unknown setting type");
            }

            public static float GetRoadWidth(roadWidth width)
            {
                int mult = 1;
                if (width == roadWidth.Одна_полоса)
                {
                    mult = 1;
                }
                else if (width == roadWidth.Две_полосы)
                {
                    mult = 2;
                }
                else if (width == roadWidth.Три_полосы)
                {
                    mult = 3;
                }
                else if (width == roadWidth.Четыре_полосы)
                {
                    mult = 4;
                }
                else if (width == roadWidth.Пять_полос)
                {
                    mult = 5;
                }
                else if (width == roadWidth.Шесть_полос)
                {
                    mult = 6;
                }
                else if (width == roadWidth.Восемь_полос)
                {
                    mult = 8;
                }
                else if (width == roadWidth.Десять_полос)
                {
                    mult = 10;
                }
                return 3f * mult;
            }

            public static void SetItemSetting(settingItemType type, bool newValue, Movable item)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        if (SettingsManager.GetSettingType(settingItem) == containerType.BoolContainer)
                        {
                            BoolContainer container = (BoolContainer)settingItem;
                            container.value = newValue;
                            return;
                        }
                    }
                }            
                throw new RAI_Exceptions.RAI_Exception.NoSettingException("WrongContainerType");
            }
            public static void SetItemSetting(settingItemType type, int newValue, Movable item)
            {
                foreach (var settingItem in item.objectSettings)
                {
                    if (settingItem.type == type)
                    {
                        if (SettingsManager.GetSettingType(settingItem) == containerType.EnumContainer)
                        {
                            EnumContainer container = (EnumContainer)settingItem;
                            container.value = newValue;
                            return;
                        }
                    }
                }

                throw new RAI_Exceptions.RAI_Exception.NoSettingException("WrongContainerType");
            }
        }
    }
    namespace RAI_Settings
    {
        [System.Serializable]
        public class TextData
        {
            public string Name;
            public string Version;
            public string Description;
            public string VideoLink;
            public string VKComment;
            public string FBComment;
            public string INSTComment;
            public string SiteLink;
            public override string ToString()
            {
                return Name + "_" + Version + "_" + Description + "_" + VideoLink + "_" + VKComment + "_" + FBComment + "_" + INSTComment + "_" + SiteLink;
            }
        }

        public enum SettingItemType
        {
            cameraUpsideDown,
            cameraMoveSpeed,
            cameraViewSize,

            vegetationRandom,
            vegetationNonstopCreating
        }

        [System.Serializable]

        public class RAI_MainSettingsContainer
        {
            public bool cameraUpsideDown = false;
            public float cameraSpeed = 10;
            public float cameraViewSize = 40;

            public bool vegetationRandom = true;
            public bool vegetationNonstopCreating = false;
        }

        /*

        [System.Serializable]
        public class SettingContainer
        {
            public SettingItemType type;
            public float value;

            public virtual SettingContainer GetValue() 
            {
                Debug.Log("BASE");
                return this;
            }
        }
        [System.Serializable]
        public class BoolContainer : SettingContainer
        {
            new public bool value;

            public override SettingContainer GetValue()
            {
                Debug.Log("VIRT");
                //base.GetValue();
                return this;
            }
        }
        [System.Serializable]
        public class FloatContainer : SettingContainer
        {
            new public float value;

            public override SettingContainer GetValue()
            {
                Debug.Log("VIRT");
                //base.GetValue();
                return this;
            }
        }
        */

        [System.Serializable]
        public class RAI_RenderSettingsContainer
        {
            public bool fxEnabled = true;
            public bool isometric = false;
            public float viewSize = 60;
            public float brightness = 0;
            public float contrast = 0;
            public float occlusion = 0;
            public float saturation = 0;
            public float temperature = 0;
        }
    }

    namespace RAI_Exceptions
    {
        public class RAI_Exception
        {
            public class NoSettingException : System.Exception
            {
                public NoSettingException(string message)
                {
                    RAI_DebugManager.instance.ShowMessage("NoSettingException [" + message + "]", Color.red);
                }
            }

            public class NoSignalException : System.Exception
            {
                public NoSignalException(string message)
                {
                    RAI_DebugManager.instance.ShowMessage("NoSignalException [" + message + "]", Color.red);
                }
            }

            public class ErrorException : System.Exception
            {
                public ErrorException(string message)
                {
                    RAI_DebugManager.instance.ShowMessage("Exception [" + message + "]", Color.red);
                }
            }
        }
    }
}

namespace RAI_Unity
{
    static class ImageManager
    {
        public static Texture2D toTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }
    }
    static class Editor
    {
        #if UNITY_EDITOR

        [MenuItem("RAI/SetMenuItems/Main")]
        static void Editor_SetMain()
        {
            GameObject.FindObjectOfType<MainUIManager>().SetToolbarItem(RoadAccidentIllustrator.RAI_UI.ToolbarItemType.MainPage);
        }

        [MenuItem("RAI/SetMenuItems/ObjectList")]
        static void Editor_SetObjectList()
        {
            GameObject.FindObjectOfType<MainUIManager>().SetToolbarItem(RoadAccidentIllustrator.RAI_UI.ToolbarItemType.AddObject);
        }

        [MenuItem("RAI/SetMenuItems/Render")]
        static void Editor_SetRender()
        {
            GameObject.FindObjectOfType<MainUIManager>().SetToolbarItem(RoadAccidentIllustrator.RAI_UI.ToolbarItemType.RenderSettings);
        }

        [MenuItem("RAI/SetMenuItems/RenderSettings")]
        static void Editor_SetRenderSettings()
        {
            GameObject.FindObjectOfType<MainUIManager>().SetToolbarItem(RoadAccidentIllustrator.RAI_UI.ToolbarItemType.VisualizationSettings);
        }

        [MenuItem("RAI/SetMenuItems/MainSettings")]
        static void Editor_SetMainSettings()
        {
            GameObject.FindObjectOfType<MainUIManager>().SetToolbarItem(RoadAccidentIllustrator.RAI_UI.ToolbarItemType.MainSettings);
        }

        [MenuItem("RAI/SetMenuItems/DevStuff")]
        static void Editor_SetDevStuff()
        {
            GameObject.FindObjectOfType<MainUIManager>().SetToolbarItem(RoadAccidentIllustrator.RAI_UI.ToolbarItemType.DevStuff);
        }

        [MenuItem("RAI/SetMenuItems/ObjectSettings")]
        static void Editor_SetList()
        {
            GameObject.FindObjectOfType<MainUIManager>().SetToolbarItem(RoadAccidentIllustrator.RAI_UI.ToolbarItemType.ObjectSettings);
        }

        [MenuItem("RAI/SetMenuItems/Acc")]
        static void Editor_SetAcc()
        {
            GameObject.FindObjectOfType<MainUIManager>().SetToolbarItem(RoadAccidentIllustrator.RAI_UI.ToolbarItemType.Account);
        }

#endif
    }
}