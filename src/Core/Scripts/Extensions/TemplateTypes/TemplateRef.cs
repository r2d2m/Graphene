﻿
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphene.Elements
{
  public class TemplateRef : BindableElement, IBindableElement<ControlType>, IGrapheneElement
  {
    /// <summary>
    /// Instantiates a <see cref="id"/> using the data read from a UXML file.
    /// </summary>
    public new class UxmlFactory : UxmlFactory<TemplateRef, UxmlTraits> { }

    /// <summary>
    /// Defines <see cref="UxmlTraits"/> for the <see cref="type"/>.
    /// </summary>
    public new class UxmlTraits : BindableElement.UxmlTraits
    {
      UxmlEnumAttributeDescription<ControlType> m_Value = new UxmlEnumAttributeDescription<ControlType> { name = "type" };

      /// <summary>
      /// Initialize <see cref="id"/> properties using values from the attribute bag.
      /// </summary>
      /// <param name="ve">The object to initialize.</param>
      /// <param name="bag">The attribute bag.</param>
      /// <param name="cc">The creation context; unused.</param>
      public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
      {
        base.Init(ve, bag, cc);
        ControlType type = m_Value.GetValueFromBag(bag, cc);

        // Only override if defined
        if (type != ControlType.None)
          ((TemplateRef)ve).type = type;
      }
    }

    private GrapheneRoot root;
    protected VisualElement m_ChildTemplate;
    protected Renderer renderer;
    
    [SerializeField]
    protected ControlType m_Type = ControlType.None;
    public virtual ControlType type
    {
      get { return m_Type; }
      set
      {
        m_Type = value;

        Render();
      }
    }

    /// <summary>
    /// USS class name of elements of this type.
    /// </summary>
    /// <remarks>
    /// Unity adds this USS class to every instance of the Template element. Any styling applied to
    /// this class affects every button located beside, or below the stylesheet in the visual tree.
    /// </remarks>
    public static readonly string ussClassName = "gr-template-ref";

    /// <summary>
    /// Constructs an Template.
    /// </summary>
    public TemplateRef() : this(null)
    {
    }

    public TemplateRef(Renderer renderer)
    {
      AddToClassList(ussClassName);
      this.renderer = renderer;
    }

    public void Inject(GrapheneRoot root, Plate plate, Renderer renderer)
    {
      this.renderer = renderer;
    }

    public void OnModelChange(ControlType newValue)
    {
      type = newValue;
    }

    void InstantiateTemplate()
    {
      TemplateAsset template = renderer.Templates.TryGetTemplateAsset(this.type);
      var clone = template.Instantiate();

      // Transfer binding path to top-level children & custom classes
      if (!string.IsNullOrWhiteSpace(bindingPath))
      {
        clone.Query<TemplateContainer>().Children<BindableElement>().ForEach(x =>
        {
          x.bindingPath = bindingPath;
          x.AddMultipleToClassList(this.GetClasses().Where(c => c != ussClassName));
        }
        );
      }

      Add(clone);
    }

    public void Render()
    {
      if (m_ChildTemplate != null)
        ClearTemplate();

      if (!renderer)
        return;

      // Clear 
      InstantiateTemplate();
    }

    void ClearTemplate()
    {
      Remove(m_ChildTemplate);
    }
  }
}