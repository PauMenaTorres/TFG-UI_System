#include "ModularMenu.h"
#include "Components/CanvasPanel.h"
#include "Components/CanvasPanelSlot.h"
#include "Components/GridPanel.h"
#include "Components/GridSlot.h"
#include "Components/Button.h"
#include "Components/TextBlock.h"
#include "Components/Slider.h"
#include "Components/CheckBox.h"
#include "Components/Image.h"
#include "Components/Spacer.h"
#include "Components/SizeBox.h"
#include "Blueprint/WidgetTree.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetSystemLibrary.h"
#include "Engine/Texture2D.h"

void UModularMenu::NativePreConstruct()
{
    Super::NativePreConstruct();

    if (!CanvasPanel)
    {
        BuildWidgetTree();
    }

    for (int32 i = 0; i < MenuElements.Num(); i++)
    {
        if (MenuElements[i].Row == -1)
        {
            MenuElements[i].Row = i;
        }
    }

    SetupBackground();
    PopulateMenu();
}

void UModularMenu::SynchronizeProperties()
{
    Super::SynchronizeProperties();
    PopulateMenu();
}

void UModularMenu::BuildWidgetTree()
{
    CanvasPanel = WidgetTree->ConstructWidget<UCanvasPanel>(UCanvasPanel::StaticClass(), TEXT("CanvasPanel"));
    WidgetTree->RootWidget = CanvasPanel;

    BackgroundWidget = WidgetTree->ConstructWidget<UImage>(UImage::StaticClass(), TEXT("BackgroundWidget"));
    UCanvasPanelSlot* BgSlot = CanvasPanel->AddChildToCanvas(BackgroundWidget);
    BgSlot->SetAnchors(FAnchors(0.0f, 0.0f, 1.0f, 1.0f));
    BgSlot->SetOffsets(FMargin(0.0f));

    GridPanel = WidgetTree->ConstructWidget<UGridPanel>(UGridPanel::StaticClass(), TEXT("GridPanel"));
    UCanvasPanelSlot* GSlot = CanvasPanel->AddChildToCanvas(GridPanel);
    GSlot->SetAutoSize(true);
}

void UModularMenu::SetupBackground()
{
    if (BackgroundWidget)
    {
        if (MenuTheme && MenuTheme->BackgroundImage)
        {
            BackgroundWidget->SetBrushFromTexture(MenuTheme->BackgroundImage);
        }
        else if (BackgroundImage)
        {
            BackgroundWidget->SetBrushFromTexture(BackgroundImage);
        }
    }
}

void UModularMenu::PopulateMenu()
{
    if (!GridPanel || !CanvasPanel) return;

    // Update Menu Alignment
    if (UCanvasPanelSlot* GSlot = Cast<UCanvasPanelSlot>(GridPanel->Slot))
    {
        FVector2D Anchor(0.5f, 0.5f);
        FVector2D Alignment(0.5f, 0.5f);

        switch (MenuHorizontalAlignment)
        {
        case HAlign_Left: Anchor.X = 0.0f; Alignment.X = 0.0f; break;
        case HAlign_Center: Anchor.X = 0.5f; Alignment.X = 0.5f; break;
        case HAlign_Right: Anchor.X = 1.0f; Alignment.X = 1.0f; break;
        case HAlign_Fill: Anchor.X = 0.5f; Alignment.X = 0.5f; break; // Fill not supported for point anchoring
        }

        switch (MenuVerticalAlignment)
        {
        case VAlign_Top: Anchor.Y = 0.0f; Alignment.Y = 0.0f; break;
        case VAlign_Center: Anchor.Y = 0.5f; Alignment.Y = 0.5f; break;
        case VAlign_Bottom: Anchor.Y = 1.0f; Alignment.Y = 1.0f; break;
        case VAlign_Fill: Anchor.Y = 0.5f; Alignment.Y = 0.5f; break;
        }

        GSlot->SetAnchors(FAnchors(Anchor.X, Anchor.Y));
        GSlot->SetAlignment(Alignment);
        GSlot->SetAutoSize(true);
        GSlot->SetOffsets(FMargin(0.0f));
    }

    TArray<UWidget*> Children = GridPanel->GetAllChildren();
    for (UWidget* Child : Children)
    {
        GridPanel->RemoveChild(Child);
        WidgetTree->RemoveWidget(Child);
    }
    ButtonConfigMap.Empty();

    for (int32 i = 0; i < MenuElements.Num(); i++)
    {
        const FUIElementConfig& Element = MenuElements[i];

        switch (Element.Type)
        {
        case EUIElementType::Button:
            CreateButton(Element, Element.Row);
            break;
        case EUIElementType::Slider:
            CreateSlider(Element, Element.Row);
            break;
        case EUIElementType::Toggle:
            CreateToggle(Element, Element.Row);
            break;
        case EUIElementType::Label:
            CreateLabel(Element, Element.Row);
            break;
        case EUIElementType::Image:
            CreateImage(Element, Element.Row);
            break;
        case EUIElementType::Spacer:
            CreateSpacer(Element, Element.Row);
            break;
        }
    }
}

void UModularMenu::AddToGrid(UWidget* Widget, const FUIElementConfig& Config, int32 AutoRow, FVector2D Size)
{
    UWidget* WidgetToAdd = Widget;

    if (Size.X > 0 || Size.Y > 0)
    {
        USizeBox* SizeBox = WidgetTree->ConstructWidget<USizeBox>();
        if (Size.X > 0)
            SizeBox->SetWidthOverride(Size.X);
        if (Size.Y > 0)
            SizeBox->SetHeightOverride(Size.Y);
        SizeBox->AddChild(Widget);
        WidgetToAdd = SizeBox;
    }

    GridPanel->AddChild(WidgetToAdd);
    if (UGridSlot* GSlot = Cast<UGridSlot>(WidgetToAdd->Slot))
    {
        GSlot->SetRow(AutoRow);
        GSlot->SetColumn(Config.Column);
        GSlot->SetRowSpan(Config.RowSpan);
        GSlot->SetColumnSpan(Config.ColumnSpan);
        GSlot->SetPadding(Config.Padding);
        GSlot->SetHorizontalAlignment(Config.HorizontalAlignment);
        GSlot->SetVerticalAlignment(Config.VerticalAlignment);
    }
}

void UModularMenu::CreateButton(const FUIElementConfig& Config, int32 AutoRow)
{
    UButton* Button = WidgetTree->ConstructWidget<UButton>();
    UTextBlock* Label = WidgetTree->ConstructWidget<UTextBlock>();

    Label->SetText(Config.ButtonText);

    FSlateColor TextColor = FSlateColor(Config.ButtonTextColor);
    int32 FontSize = Config.ButtonFontSize;
    FVector2D BtnSize = Config.ButtonSize;
    UTexture2D* NormalTex = Config.ButtonNormalTexture;
    UTexture2D* HoveredTex = Config.ButtonHoveredTexture;
    UTexture2D* PressedTex = Config.ButtonPressedTexture;

    if (MenuTheme)
    {
        TextColor = FSlateColor(MenuTheme->ButtonTextColor);
        FontSize = MenuTheme->ButtonFontSize;
        BtnSize = MenuTheme->ButtonSize;
        if (MenuTheme->ButtonNormalTexture) NormalTex = MenuTheme->ButtonNormalTexture;
        if (MenuTheme->ButtonHoveredTexture) HoveredTex = MenuTheme->ButtonHoveredTexture;
        if (MenuTheme->ButtonPressedTexture) PressedTex = MenuTheme->ButtonPressedTexture;
    }

    Label->SetColorAndOpacity(TextColor);

    FSlateFontInfo Font = Label->GetFont();
    Font.Size = FontSize;
    Label->SetFont(Font);

    Button->AddChild(Label);

    FButtonStyle BtnStyle = Button->GetStyle();
    BtnStyle.Normal.ImageSize = BtnSize;
    BtnStyle.Hovered.ImageSize = BtnSize;
    BtnStyle.Pressed.ImageSize = BtnSize;

    if (NormalTex)
    {
        BtnStyle.Normal.SetResourceObject(NormalTex);
        BtnStyle.Normal.DrawAs = ESlateBrushDrawType::Image;
    }
    if (HoveredTex)
    {
        BtnStyle.Hovered.SetResourceObject(HoveredTex);
        BtnStyle.Hovered.DrawAs = ESlateBrushDrawType::Image;
    }
    if (PressedTex)
    {
        BtnStyle.Pressed.SetResourceObject(PressedTex);
        BtnStyle.Pressed.DrawAs = ESlateBrushDrawType::Image;
    }
    Button->SetStyle(BtnStyle);

    ButtonConfigMap.Add(Button, Config);
    Button->OnClicked.AddDynamic(this, &UModularMenu::OnDynamicButtonClicked);

    AddToGrid(Button, Config, AutoRow, BtnSize);
}

void UModularMenu::CreateSlider(const FUIElementConfig& Config, int32 AutoRow)
{
    UTextBlock* Label = WidgetTree->ConstructWidget<UTextBlock>();
    Label->SetText(Config.SliderLabel);
    Label->SetColorAndOpacity(FSlateColor(Config.SliderLabelColor));

    FSlateFontInfo LabelFont = Label->GetFont();
    LabelFont.Size = Config.SliderLabelFontSize;
    Label->SetFont(LabelFont);

    AddToGrid(Label, Config, AutoRow, FVector2D(0.0f, 0.0f));

    USlider* Slider = WidgetTree->ConstructWidget<USlider>();
    Slider->SetMinValue(Config.SliderMin);
    Slider->SetMaxValue(Config.SliderMax);
    Slider->SetValue(Config.SliderDefault);
    Slider->SetStepSize(Config.SliderStepSize);
    Slider->SetSliderBarColor(Config.SliderBarColor);
    Slider->SetSliderHandleColor(Config.SliderHandleColor);

    FUIElementConfig SliderSlotConfig = Config;
    SliderSlotConfig.Column = Config.Column + 1;
    AddToGrid(Slider, SliderSlotConfig, AutoRow, Config.SliderSize);
}

void UModularMenu::CreateToggle(const FUIElementConfig& Config, int32 AutoRow)
{
    UCheckBox* CheckBox = WidgetTree->ConstructWidget<UCheckBox>();
    CheckBox->SetIsChecked(Config.bToggleDefault);

    if (Config.ToggleCheckedImage)
    {
        CheckBox->WidgetStyle.CheckedImage.SetResourceObject(Config.ToggleCheckedImage);
        CheckBox->WidgetStyle.CheckedHoveredImage.SetResourceObject(Config.ToggleCheckedImage);
        CheckBox->WidgetStyle.CheckedPressedImage.SetResourceObject(Config.ToggleCheckedImage);
    }
    if (Config.ToggleUncheckedImage)
    {
        CheckBox->WidgetStyle.UncheckedImage.SetResourceObject(Config.ToggleUncheckedImage);
        CheckBox->WidgetStyle.UncheckedHoveredImage.SetResourceObject(Config.ToggleUncheckedImage);
        CheckBox->WidgetStyle.UncheckedPressedImage.SetResourceObject(Config.ToggleUncheckedImage);
    }

    AddToGrid(CheckBox, Config, AutoRow, FVector2D(0.0f, 0.0f));

    UTextBlock* Label = WidgetTree->ConstructWidget<UTextBlock>();
    Label->SetText(Config.ToggleLabel);
    Label->SetColorAndOpacity(FSlateColor(Config.ToggleLabelColor));

    FSlateFontInfo Font = Label->GetFont();
    Font.Size = Config.ToggleLabelFontSize;
    Label->SetFont(Font);

    FUIElementConfig LabelSlotConfig = Config;
    LabelSlotConfig.Column = Config.Column + 1;
    LabelSlotConfig.VerticalAlignment = VAlign_Center;
    AddToGrid(Label, LabelSlotConfig, AutoRow, Config.ToggleSize);
}

void UModularMenu::CreateLabel(const FUIElementConfig& Config, int32 AutoRow)
{
    UTextBlock* TextBlock = WidgetTree->ConstructWidget<UTextBlock>();
    TextBlock->SetText(Config.LabelText);
    
    FSlateColor TextColor = FSlateColor(Config.LabelTextColor);
    int32 FontSize = Config.LabelFontSize;

    if (MenuTheme)
    {
        TextColor = FSlateColor(MenuTheme->LabelTextColor);
        FontSize = MenuTheme->LabelFontSize;
    }

    TextBlock->SetColorAndOpacity(TextColor);

    FSlateFontInfo Font = TextBlock->GetFont();
    Font.Size = FontSize;
    TextBlock->SetFont(Font);

    AddToGrid(TextBlock, Config, AutoRow, Config.LabelSize);
}

void UModularMenu::CreateImage(const FUIElementConfig& Config, int32 AutoRow)
{
    UImage* ImageWidget = WidgetTree->ConstructWidget<UImage>();

    if (Config.ElementImage)
    {
        ImageWidget->SetBrushFromTexture(Config.ElementImage);
    }

    ImageWidget->SetDesiredSizeOverride(Config.ImageSize);
    ImageWidget->SetColorAndOpacity(Config.ImageTint);

    AddToGrid(ImageWidget, Config, AutoRow, Config.ImageSize);
}

void UModularMenu::CreateSpacer(const FUIElementConfig& Config, int32 AutoRow)
{
    USpacer* SpacerWidget = WidgetTree->ConstructWidget<USpacer>();
    SpacerWidget->SetSize(FVector2D(0.0f, Config.SpacerHeight));

    AddToGrid(SpacerWidget, Config, AutoRow, FVector2D(0.0f, Config.SpacerHeight));
}

void UModularMenu::OnDynamicButtonClicked()
{
    for (const auto& Pair : ButtonConfigMap)
    {
        UButton* Button = Pair.Key;
        const FUIElementConfig& Config = Pair.Value;

        if (Button->IsHovered())
        {
            switch (Config.ButtonAction)
            {
            case EUIButtonAction::OpenLevel:
                UGameplayStatics::OpenLevel(GetWorld(), Config.TargetLevelName);
                break;

            case EUIButtonAction::QuitGame:
                UKismetSystemLibrary::QuitGame(this, nullptr, EQuitPreference::Quit, false);
                break;
            }
            break;
        }
    }
}
