using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorLookup
{
    public struct CustomColor
    {
        public string Name;
        public string Hex;
        public int Cost;
        public List<CustomColor> SubShades;

        public CustomColor(string _Name, string _Hex, int _Cost,List<CustomColor> _SubShades)
        {
            Name = _Name;
            Hex = _Hex;
            Cost = _Cost;
            SubShades = _SubShades;
        }
    }

    public static List<CustomColor> CustomColors = new List<CustomColor>()
    {
        new CustomColor("Black","373254", 75, new List<CustomColor>()
        {
            new CustomColor("Dim Black","2F2B49", 60, null),
            new CustomColor("Dark Black","292540", 50, null),
            new CustomColor("Darker Black","231F37", 40, null),
            new CustomColor("Darkest Black","1B182C", 30, null)
        }),
        new CustomColor("Purple","68356F", 60, new List<CustomColor>()
        {
            new CustomColor("Dim Purple","5B2E61", 50, null),
            new CustomColor("Dark Purple","502755", 40, null),
            new CustomColor("Darker Purple","45214A", 30, null),
            new CustomColor("Darkest Purple","381A3C", 20, null)
        }),
        new CustomColor("Gray","5E6B82", 40, new List<CustomColor>()
        {
            new CustomColor("Dim Gray","525E72", 30, null),
            new CustomColor("Dark Gray","485264", 20, null),
            new CustomColor("Darker Gray","3E4858", 20, null),
            new CustomColor("Darkest Gray","323A48", 20, null)
        }),
        new CustomColor("Indigo","25718C", 20, new List<CustomColor>()
        {
            new CustomColor("Dim Indigo","1F637B", 20, null),
            new CustomColor("Dark Indigo","1A576C", 20, null),
            new CustomColor("Darker Indigo","164C5F", 20, null),
            new CustomColor("Darkest Indigo","103E4E", 20, null)
        }),
        new CustomColor("Blue","11ABBE", 20, new List<CustomColor>()
        {
            new CustomColor("Dim Blue","0E97A8", 20, null),
            new CustomColor("Dark Blue","0B8594", 20, null),
            new CustomColor("Darker Blue","087582", 20, null),
            new CustomColor("Darkest Blue","05606B", 20, null)
        }),
        new CustomColor("Sea Foam","69F6BF", 60, new List<CustomColor>()
        {
            new CustomColor("Dim Sea Foam","5CDAA9", 50, null),
            new CustomColor("Dark Sea Foam","50C095", 40, null),
            new CustomColor("Darker Sea Foam","46AA83", 30, null),
            new CustomColor("Darkest Sea Foam","398D6C", 20, null)
        }),
        new CustomColor("White","EFF0D7", 75, new List<CustomColor>()
        {
            new CustomColor("Dim White","D3D4BE", 60, null),
            new CustomColor("Dark White","BBBCA8", 50, null),
            new CustomColor("Darker White","A5A694", 40, null),
            new CustomColor("Darkest White","88897A", 30, null)
        }),
        new CustomColor("Yellow","F8E574", 20, new List<CustomColor>()
        {
            new CustomColor("Dim Yellow","DBCA66", 20, null),
            new CustomColor("Dark Yellow","C2B359", 20, null),
            new CustomColor("Darker Yellow","AB9E4E", 20, null),
            new CustomColor("Darkest Yellow","8E823F", 20, null)
        }),
        new CustomColor("Lime","A3E75C", 150, new List<CustomColor>()
        {
            new CustomColor("Dim Lime","90CC50", 140, null),
            new CustomColor("Dark Lime","7EB446", 125, null),
            new CustomColor("Darker Lime","6F9F3D", 100, null),
            new CustomColor("Darkest Lime","5B8431", 50, null)
        }),
        new CustomColor("Chocolate","6D4442", 100, new List<CustomColor>()
        {
            new CustomColor("Dim Chocolate","603B39", 120, null),
            new CustomColor("Dark Chocolate","543331", 140, null),
            new CustomColor("Darker Chocolate","492C2B", 160, null),
            new CustomColor("Darkest Chocolate","3B2322", 180, null)
        }),
        new CustomColor("Brown","A16557", 100, new List<CustomColor>()
        {
            new CustomColor("Dim Brown","8E584C", 90, null),
            new CustomColor("Dark Brown","7D4D42", 80, null),
            new CustomColor("Darker Brown","6E4339", 70, null),
            new CustomColor("Darkest Brown","5A362E", 60, null)
        }),
        new CustomColor("Pink","F98BB7", 150, new List<CustomColor>()
        {
            new CustomColor("Dim Pink","DC7AA2", 125, null),
            new CustomColor("Dark Pink","C36B8E", 100, null),
            new CustomColor("Darker Pink","AC5E7D", 75, null),
            new CustomColor("Darkest Pink","8E4D67", 50, null)
        }),
        new CustomColor("Red","C84C66", 50, new List<CustomColor>()
        {
            new CustomColor("Dim Red","B14259", 40, null),
            new CustomColor("Dark Red","9C394E", 30, null),
            new CustomColor("Darker Red","893244", 20, null),
            new CustomColor("Darkest Red","712837", 10, null)
        }),
        new CustomColor("Orange","F79152", 20, new List<CustomColor>()
        {
            new CustomColor("Dim Orange","DB8048", 20, null),
            new CustomColor("Dark Orange","C1703E", 20, null),
            new CustomColor("Darker Orange","AB6236", 20, null),
            new CustomColor("Darkest Orange","8D512B", 20, null)
        }),
        new CustomColor("Rock","9B9C82", 20, new List<CustomColor>()
        {
            new CustomColor("Dim Rock","898972", 20, null),
            new CustomColor("Dark Rock","787964", 20, null),
            new CustomColor("Darker Rock","696A58", 20, null),
            new CustomColor("Darkest Rock","565748", 20, null)
        }),
        new CustomColor("Teal","1C866D", 75, new List<CustomColor>()
        {
            new CustomColor("Dim Teal","177660", 60, null),
            new CustomColor("Dark Teal","136754", 50, null),
            new CustomColor("Darker Teal","105B49", 40, null),
            new CustomColor("Darkest Teal","0B4A3B", 30, null)
        }),
        new CustomColor("Green","59B15E", 50, new List<CustomColor>()
        {
            new CustomColor("Dim Green","4E9C52", 40, null),
            new CustomColor("Dark Green","448A48", 30, null),
            new CustomColor("Darker Green","3B793E", 20, null),
            new CustomColor("Darkest Green","2F6432", 20, null)
        })
    };
}
