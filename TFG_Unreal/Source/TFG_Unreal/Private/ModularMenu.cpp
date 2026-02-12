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
    GSlot->SetAnchors(FAnchors(0.0f, 0.0f, 1.0f, 1.0f));
    GSlot->SetOffsets(FMargin(0.0f));
}

void UModularMenu::SetupBackground()
{
    if (BackgroundWidget && BackgroundImage)
    {
        BackgroundWidget->SetBrushFromTexture(BackgroundImage);
    }
}

void UModularMenu::PopulateMenu()
{
    if (!GridPanel) return;

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
    Label->SetColorAndOpacity(FSlateColor(Config.ButtonTextColor));

    FSlateFontInfo Font = Label->GetFont();
    Font.Size = Config.ButtonFontSize;
    Label->SetFont(Font);

    Button->AddChild(Label);

    FButtonStyle BtnStyle = Button->GetStyle();
    BtnStyle.Normal.ImageSize = Config.ButtonSize;
    BtnStyle.Hovered.ImageSize = Config.ButtonSize;
    BtnStyle.Pressed.ImageSize = Config.ButtonSize;

    if (Config.ButtonNormalTexture)
    {
        BtnStyle.Normal.SetResourceObject(Config.ButtonNormalTexture);
        BtnStyle.Normal.DrawAs = ESlateBrushDrawType::Image;
    }
    if (Config.ButtonHoveredTexture)
    {
        BtnStyle.Hovered.SetResourceObject(Config.ButtonHoveredTexture);
        BtnStyle.Hovered.DrawAs = ESlateBrushDrawType::Image;
    }
    if (Config.ButtonPressedTexture)
    {
        BtnStyle.Pressed.SetResourceObject(Config.ButtonPressedTexture);
        BtnStyle.Pressed.DrawAs = ESlateBrushDrawType::Image;
    }
    Button->SetStyle(BtnStyle);

    ButtonConfigMap.Add(Button, Config);
    Button->OnClicked.AddDynamic(this, &UModularMenu::OnDynamicButtonClicked);

    AddToGrid(Button, Config, AutoRow, Config.ButtonSize);
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
    TextBlock->SetColorAndOpacity(FSlateColor(Config.LabelTextColor));

    FSlateFontInfo Font = TextBlock->GetFont();
    Font.Size = Config.LabelFontSize;
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
