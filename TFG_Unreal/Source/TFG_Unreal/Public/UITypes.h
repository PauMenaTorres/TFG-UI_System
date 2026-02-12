#pragma once

#include "CoreMinimal.h"
#include "Layout/Margin.h"
#include "UITypes.generated.h"

class UTexture2D;

UENUM(BlueprintType)
enum class EUIElementType : uint8
{
    Button,
    Slider,
    Toggle,
    Label,
    Image,
    Spacer
};

UENUM(BlueprintType)
enum class EUIButtonAction : uint8
{
    OpenLevel,
    QuitGame
};

USTRUCT(BlueprintType)
struct FUIElementConfig
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Element")
    EUIElementType Type = EUIElementType::Button;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Layout")
    int32 Row = -1;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Layout")
    int32 Column = 0;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Layout")
    int32 RowSpan = 1;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Layout")
    int32 ColumnSpan = 1;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Layout")
    FMargin Padding = FMargin(4.0f);

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Layout")
    TEnumAsByte<EHorizontalAlignment> HorizontalAlignment = HAlign_Center;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Layout")
    TEnumAsByte<EVerticalAlignment> VerticalAlignment = VAlign_Center;



    // Button
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button",
        meta = (EditCondition = "Type == EUIElementType::Button", EditConditionHides))
    FText ButtonText;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button",
        meta = (EditCondition = "Type == EUIElementType::Button", EditConditionHides))
    EUIButtonAction ButtonAction = EUIButtonAction::OpenLevel;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button",
        meta = (EditCondition = "Type == EUIElementType::Button && ButtonAction == EUIButtonAction::OpenLevel", EditConditionHides))
    FName TargetLevelName;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button|Style",
        meta = (EditCondition = "Type == EUIElementType::Button", EditConditionHides))
    FVector2D ButtonSize = FVector2D(200.0f, 50.0f);

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button|Style",
        meta = (EditCondition = "Type == EUIElementType::Button", EditConditionHides))
    FLinearColor ButtonTextColor = FLinearColor::White;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button|Style",
        meta = (EditCondition = "Type == EUIElementType::Button", EditConditionHides))
    int32 ButtonFontSize = 16;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button|Style",
        meta = (EditCondition = "Type == EUIElementType::Button", EditConditionHides))
    UTexture2D* ButtonNormalTexture = nullptr;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button|Style",
        meta = (EditCondition = "Type == EUIElementType::Button", EditConditionHides))
    UTexture2D* ButtonHoveredTexture = nullptr;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Button|Style",
        meta = (EditCondition = "Type == EUIElementType::Button", EditConditionHides))
    UTexture2D* ButtonPressedTexture = nullptr;

    // Slider
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    FText SliderLabel;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    float SliderMin = 0.0f;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    float SliderMax = 1.0f;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    float SliderDefault = 0.5f;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider|Style",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    FLinearColor SliderBarColor = FLinearColor::White;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider|Style",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    FLinearColor SliderHandleColor = FLinearColor::White;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider|Style",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    float SliderStepSize = 0.01f;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider|Style",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    FVector2D SliderSize = FVector2D(200.0f, 40.0f);

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider|Style",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    int32 SliderLabelFontSize = 14;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Slider|Style",
        meta = (EditCondition = "Type == EUIElementType::Slider", EditConditionHides))
    FLinearColor SliderLabelColor = FLinearColor::White;

    // Toggle
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Toggle",
        meta = (EditCondition = "Type == EUIElementType::Toggle", EditConditionHides))
    FText ToggleLabel;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Toggle",
        meta = (EditCondition = "Type == EUIElementType::Toggle", EditConditionHides))
    bool bToggleDefault = false;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Toggle|Style",
        meta = (EditCondition = "Type == EUIElementType::Toggle", EditConditionHides))
    FLinearColor ToggleLabelColor = FLinearColor::White;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Toggle|Style",
        meta = (EditCondition = "Type == EUIElementType::Toggle", EditConditionHides))
    int32 ToggleLabelFontSize = 14;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Toggle|Style",
        meta = (EditCondition = "Type == EUIElementType::Toggle", EditConditionHides))
    FVector2D ToggleSize = FVector2D(200.0f, 40.0f);

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Toggle|Style",
        meta = (EditCondition = "Type == EUIElementType::Toggle", EditConditionHides))
    UTexture2D* ToggleCheckedImage = nullptr;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Toggle|Style",
        meta = (EditCondition = "Type == EUIElementType::Toggle", EditConditionHides))
    UTexture2D* ToggleUncheckedImage = nullptr;

    // Label
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Label",
        meta = (EditCondition = "Type == EUIElementType::Label", EditConditionHides))
    FText LabelText;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Label|Style",
        meta = (EditCondition = "Type == EUIElementType::Label", EditConditionHides))
    FLinearColor LabelTextColor = FLinearColor::White;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Label|Style",
        meta = (EditCondition = "Type == EUIElementType::Label", EditConditionHides))
    int32 LabelFontSize = 16;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Label|Style",
        meta = (EditCondition = "Type == EUIElementType::Label", EditConditionHides))
    FVector2D LabelSize = FVector2D(0.0f, 0.0f);

    // Image
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Image",
        meta = (EditCondition = "Type == EUIElementType::Image", EditConditionHides))
    UTexture2D* ElementImage = nullptr;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Image",
        meta = (EditCondition = "Type == EUIElementType::Image", EditConditionHides))
    FVector2D ImageSize = FVector2D(100.0f, 100.0f);

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Image|Style",
        meta = (EditCondition = "Type == EUIElementType::Image", EditConditionHides))
    FLinearColor ImageTint = FLinearColor::White;

    // Spacer
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Spacer",
        meta = (EditCondition = "Type == EUIElementType::Spacer", EditConditionHides))
    float SpacerHeight = 20.0f;
};

UCLASS(BlueprintType)
class TFG_UNREAL_API UUITheme : public UDataAsset
{
    GENERATED_BODY()

public:
    // Button Style
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|Button")
    UTexture2D* ButtonNormalTexture = nullptr;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|Button")
    UTexture2D* ButtonHoveredTexture = nullptr;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|Button")
    UTexture2D* ButtonPressedTexture = nullptr;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|Button")
    FLinearColor ButtonTextColor = FLinearColor::White;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|Button")
    int32 ButtonFontSize = 24;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|Button")
    FVector2D ButtonSize = FVector2D(300.0f, 60.0f);

    // Label Style
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|Label")
    FLinearColor LabelTextColor = FLinearColor::White;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|Label")
    int32 LabelFontSize = 48;

    // General
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Theme|General")
    UTexture2D* BackgroundImage = nullptr;
};