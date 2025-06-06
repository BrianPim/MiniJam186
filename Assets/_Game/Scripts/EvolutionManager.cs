using System.Collections.Generic;
using CamLib;
using System.Linq;
using UnityEngine;

public class EvolutionManager : Singleton<EvolutionManager>
{
    public List<EvolutionInstance> _elements = new List<EvolutionInstance>();

    protected override void Awake()
    {
        base.Awake();
        Add(Element.Normal);
        _elements[0].Value = 1;
        
        Add(Element.Fire);
        Add(Element.Ice);
        Add(Element.Electric);
        
        EvaluateNormalizedWeights();
    }

    public void Add(Element element)
    {
        EvolutionInstance inst = new EvolutionInstance
        {
            Element = element,
        };
        _elements.Add(inst);
    }
    
    public class EvolutionInstance
    {
        public int Value;
        public Element Element;
        public float NormalizedWeight;
    }

    [ContextMenu("AddNormal")]
    public void AddNormal() => Increment(Element.Normal);
    [ContextMenu("AddFire")]
    public void AddFire() => Increment(Element.Fire);
    [ContextMenu("AddIce")]
    public void AddIce() => Increment(Element.Ice);
    [ContextMenu("AddElectric")]
    public void AddElectric() => Increment(Element.Electric);
    
    public void Increment(Element element, int value = 1)
    {
        EvolutionInstance evo = _elements.Find(p => p.Element == element);
        evo.Value++;
        EvaluateNormalizedWeights();
    }
    
    public void EvaluateNormalizedWeights()
    {
        float totalSum = _elements.Sum(e => e.Value);

        if (totalSum <= 0)
        {
            float equalWeight = 1f / _elements.Count;
            foreach (var element in _elements)
            {
                element.NormalizedWeight = equalWeight;
            }
            return;
        }

        foreach (var element in _elements)
        {
            element.NormalizedWeight = element.Value / totalSum;
        }

        float[] weightsForUi = _elements.Select(p => p.NormalizedWeight).ToArray();
        HudBarEvolution.Instance.UpdateBars(weightsForUi);
    }

    public Element GetRandomElementalType()
    {
        if (_elements.IsNullOrEmpty())
        {
            return Element.Normal;
        }

        float randomValue = Random.value;
        float cumulativeWeight = 0f;

        foreach (var evolutionInstance in _elements)
        {
            cumulativeWeight += evolutionInstance.NormalizedWeight;
        
            if (randomValue <= cumulativeWeight)
            {
                return evolutionInstance.Element;
            }
        }
        
        return _elements[0].Element;
    }
}