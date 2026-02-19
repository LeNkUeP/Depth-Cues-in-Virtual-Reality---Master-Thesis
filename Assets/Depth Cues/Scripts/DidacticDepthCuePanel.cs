using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class DidacticDepthCuePanel : MonoBehaviour
{
    public GameObject didacticUI;
    public TextMeshProUGUI currentCueHeading;
    public TextMeshProUGUI currentCueExplanation;

    [Header("Binocular disparity - Settings")]
    public GameObject binocularDisparityVideo;
    private string binocularDisparityHeading = "Binokulare Disparität";
    private string binocularDisparityExplanation = "Binokulare Disparität ist ein Tiefenhinweis, der entsteht, " +
        "weil unsere beiden Augen die Welt aus leicht unterschiedlichen Blickwinkeln sehen." +
        "Da die Augen etwa 6–7 cm voneinander entfernt sind, entsteht auf jeder Netzhaut ein minimal versetztes Bild desselben Objekts." +
        "Das Gehirn vergleicht diese beiden Bilder und berechnet aus dem horizontalen Unterschied die Entfernung des Objekts.";

    [Header("Motion parallax - Settings")]
    public GameObject motionParallaxVideo;
    private string motionParallaxHeading = "Bewegungsparallaxe";
    private string motionParallaxExplanation = "Bei seitlicher Kopfbewegung verschieben sich nahe Objekte schneller über die Netzhaut " +
        "als entfernte Objekte. Diese unterschiedliche Winkelgeschwindigkeit liefert Information über relative Tiefenabstände. " +
        "Kopfbewegungen können zusätzliche Tiefeninformation vermitteln.";

    [Header("Accretion - Settings")]
    public GameObject accretionVideo;
    private string accretionHeading = "Akkretion";
    private string accretionExplanation = "Bewegt sich der Beobachter oder ein Objekt, werden zuvor verdeckte Bereiche " +
        "eines Hintergrundobjekts sukzessive sichtbar (Deletion und Accretion). Diese graduelle Veränderung liefert Information " +
        "über Tiefenstaffelung zwischen Vorder- und Hintergrund.";

    [Header("Occlusion - Settings")]
    public GameObject occlusionVideo;
    private string occlusionHeading = "Verdeckung";
    private string occlusionExplanation = "Wird ein Objekt von einem anderen Objekt verdeckt, befindet es sich dahinter bzw. " +
        "weiter vom Betrachter entfernt als das verdeckende Objekt.";

    [Header("Accommodation - Settings")]
    public GameObject accommodationVideo;
    private string accommodationHeading = "Akkommodation";
    private string accommodationExplanation = "Um mein scharfes Bild auf der Netzhaut zu erhalten wird Akkommodation der Augenlinse eingesetzt. " +
        "Dabei steuern Muskeln die Krümmung der elastischen Linse abhängig von der Distanz zu dem fokussierten Objekt. " +
        "Bei Objekten in der Ferne wird die Linse flach gezogen was zu einer geringen Brechkraft führt. " +
        "Im Nahbereich wird die Linse gekrümmt und kugelförmig. In diesem Zustand hat sie eine hohe Brechkraft und kann nahe " +
        "Objekte scharf auf der Netzhaut abbilden.";

    [Header("Convergence - Settings")]
    public GameObject convergenceVideo;
    private string convergenceHeading = "Konvergenz";
    private string convergenceExplanation = "Mit Konvergenz wird in der Wahrnehmungsphysiologie die Innendrehung beider Augen bezeichnet. " +
        "Diese findet statt wenn sich ein fokussiertes Objekt das zwischen den Augen liegt, diesen annähert." +
        "Je größer die Konvergenz, bzw. der Winkel zwischen Objekt und den beiden Blickrichtungen, " +
        "desto näher befindet sich das Objekt an den Augen des Betrachters.";

    [Header("Image Blur - Settings")]
    public GameObject imageBlurVideo;
    private string imageBlurHeading = "Image Blur";
    private string imageBlurExplanation = "Sobald sich die Augenlinsen auf ein Objekt oder eine Entfernung einstellen, " +
        "entsteht eine Fokusebene im Bereich des Fixationspunktes. Objekte außerhalb der Fokusebene erscheinen unscharf " +
        "und werden als weiter entfernt oder näher interpretiert. Daher kann Unschärfe im Bild als Information über die " +
        "Entfernung von Objekten genutzt werden.";

    [Header("Atmospheric Perspective - Settings")]
    public GameObject atmosphericPerspectiveVideo;
    private string atmosphericPerspectiveHeading = "Atmosphärische Perspektive";
    private string atmosphericPerspectiveExplanation = "Mit zunehmender Entfernung werden Objekte kontrastärmer, blasser und häufig bläulicher. " +
        "Ursache ist die Streuung des Lichts in der Atmosphäre. Das visuelle System interpretiert geringeren Kontrast und reduzierte " +
        "Farbsättigung als Hinweis auf große Distanz.";

    [Header("Texture gradient - Settings")]
    public GameObject textureGradientVideo;
    private string textureGradientHeading = "Texturgradient";
    private string textureGradientExplanation = "Mit zunehmender Entfernung erscheinen Texturen dichter, " +
        "feiner und weniger detailliert. Nahegelegene Oberflächenstrukturen sind klar erkennbar, während weiter " +
        "entfernte Strukturen komprimiert wirken.";

    [Header("Linear perspective - Settings")]
    public GameObject linearPerspectiveVideo;
    private string linearPerspectiveHeading = "Lineare Perspektive";
    private string linearPerspectiveExplanation = "Linearperspektive beschreibt das Phänomen, dass parallele Linien in der Realität" +
        " mit zunehmender Entfernung scheinbar aufeinander zulaufen und sich in einem Fluchtpunkt treffen.";

    [Header("Shadow cast - Settings")]
    public GameObject shadowCastVideo;
    private string shadowCastHeading = "Schattenwurf";
    private string shadowCastExplanation = "Wirft ein Objekt einen Schatten auf eine Oberfläche oder ein anderes Objekt, " +
        "liefert dies Information über Abstand und räumliche Anordnung. Der Abstand zwischen Objekt und Schatten gibt Aufschluss " +
        "über die Höhe über dem Boden.";

    [Header("Shape from shading - Settings")]
    public GameObject shapeFromShadingVideo;
    private string shapeFromShadingHeading = "Shape from shading";
    private string shapeFromShadingExplanation = "Durch Lichtquellen entstehen Helligkeitsverläufe auf Objektoberflächen. " +
        "Das visuelle System interpretiert diese Gradienten unter Annahme einer Lichtquelle (meist von oben) als Hinweise auf Wölbungen, " +
        "Vertiefungen oder Kanten. So entsteht aus zweidimensionalen Schattierungen ein Eindruck dreidimensionaler Form.";

    [Header("Relative size - Settings")]
    public GameObject relativeSizeVideo;
    private string relativeSizeHeading = "Relative Größe";
    private string relativeSizeExplanation = "Befinden sich mehrere gleichartige Objekte im Sichtfeld, " +
        "erscheint das kleinere Objekt weiter entfernt als das größere. Dieser Hinweis funktioniert besonders zuverlässig, " +
        "wenn bekannt ist, dass die Objekte tatsächlich gleich groß sind. Die Interpretation erfolgt durch einen Vergleich " +
        "der retinalen Bildgröße.";

    [Header("Known size - Settings")]
    public GameObject knownSizeVideo;
    private string knownSizeHeading = "Bekannte Größe";
    private string knownSizeExplanation = "Ist die reale Größe eines Objekts bekannt, kann aus seiner retinalen Bildgröße " +
        "auf die Entfernung geschlossen werden. Ein Mensch beispielsweise wird bei kleiner retinaler Abbildung " +
        "als weiter entfernt interpretiert. Dieser Prozess basiert auf erlernten Größenkonstanten.";

    [Header("Height in field of view - Settings")]
    public GameObject heightInFieldOfViewVideo;
    private string heightInFieldOfViewHeading = "Höhe im Gesichtsfeld";
    private string heightInFieldOfViewExplanation = "Objekte, die näher am Horizont bzw. höher im Gesichtsfeld erscheinen, " +
        "werden als weiter entfernt interpretiert.";

    private void Start()
    {
        occlusionVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        binocularDisparityVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        convergenceVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        motionParallaxVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        heightInFieldOfViewVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        knownSizeVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        relativeSizeVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        shapeFromShadingVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        shadowCastVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        atmosphericPerspectiveVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        linearPerspectiveVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        textureGradientVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        accretionVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        imageBlurVideo.GetComponentInChildren<VideoPlayer>().Prepare();
        accommodationVideo.GetComponentInChildren<VideoPlayer>().Prepare();
    }

    public void updateDidacticUI(String heading, String explanation, GameObject video)
    {
        if (!didacticUI.GetComponent<Canvas>().enabled)
        {
            currentCueHeading.text = heading;
            currentCueExplanation.text = explanation;
            video.GetComponentInChildren<VideoPlayer>().time = 0;
            video.GetComponentInChildren<VideoPlayer>().Play();
            video.GetComponent<RectTransform>().localScale = Vector3.one;
            didacticUI.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            currentCueHeading.text = "";
            currentCueExplanation.text = "";
            video.GetComponentInChildren<VideoPlayer>().time = 0;
            video.GetComponentInChildren<VideoPlayer>().Stop();
            video.GetComponentInChildren<VideoPlayer>().Prepare();
            video.GetComponent<RectTransform>().localScale = Vector3.zero;
            didacticUI.GetComponent<Canvas>().enabled = false;
        }

    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // ACCOMMODATION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAccommodation()
    {
        updateDidacticUI(accommodationHeading, accommodationExplanation, accommodationVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // IMAGE BLUR
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleImageBlur()
    {
        updateDidacticUI(imageBlurHeading, imageBlurExplanation, imageBlurVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // HEIGHT IN FIELD OF VIEW
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleHeightInFieldOfView()
    {
        updateDidacticUI(heightInFieldOfViewHeading, heightInFieldOfViewExplanation, heightInFieldOfViewVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // KNOWN SIZE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleKnownSize()
    {
        updateDidacticUI(knownSizeHeading, knownSizeExplanation, knownSizeVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // RELATIVE SIZE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleRelativeSize()
    {
        updateDidacticUI(relativeSizeHeading, relativeSizeExplanation, relativeSizeVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // ATMOSPHERIC PERSPECTIVE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAtmosphericPerspective()
    {
        updateDidacticUI(atmosphericPerspectiveHeading, atmosphericPerspectiveExplanation, atmosphericPerspectiveVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // MOTION PARALLAX
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleMotionParallax()
    {
        updateDidacticUI(motionParallaxHeading, motionParallaxExplanation, motionParallaxVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // DISPARITY
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleDisparity()
    {
        updateDidacticUI(binocularDisparityHeading, binocularDisparityExplanation, binocularDisparityVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // CONVERGENCE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleConvergence()
    {
        updateDidacticUI(convergenceHeading, convergenceExplanation, convergenceVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // OCCLUSION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleOcclusion()
    {
        updateDidacticUI(occlusionHeading, occlusionExplanation, occlusionVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // SHADOW CAST
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleShadowCast()
    {
        updateDidacticUI(shadowCastHeading, shadowCastExplanation, shadowCastVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // SHAPE FROM SHADING
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleShapeFromShading()
    {
        updateDidacticUI(shapeFromShadingHeading, shapeFromShadingExplanation, shapeFromShadingVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // LINEARER PERSPECTIVE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleLinearPerspective()
    {
        updateDidacticUI(linearPerspectiveHeading, linearPerspectiveExplanation, linearPerspectiveVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // TEXTURE GRADIENT
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleTextureGradient()
    {
        updateDidacticUI(textureGradientHeading, textureGradientExplanation, textureGradientVideo);
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // ACCRETION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAccretion()
    {
        updateDidacticUI(accretionHeading, accretionExplanation, accretionVideo);
    }
}
