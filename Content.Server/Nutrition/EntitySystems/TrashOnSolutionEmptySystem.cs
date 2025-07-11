using Content.Server.Nutrition.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Server.Nutrition.EntitySystems
{
    public sealed class TrashOnSolutionEmptySystem : EntitySystem
    {
        [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;
        [Dependency] private readonly TagSystem _tagSystem = default!;

        private static readonly ProtoId<TagPrototype> TrashTag = "Trash";

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<TrashOnSolutionEmptyComponent, MapInitEvent>(OnMapInit);
            SubscribeLocalEvent<TrashOnSolutionEmptyComponent, SolutionContainerChangedEvent>(OnSolutionChange);
        }

        public void OnMapInit(Entity<TrashOnSolutionEmptyComponent> entity, ref MapInitEvent args)
        {
            CheckSolutions(entity);
        }

        public void OnSolutionChange(Entity<TrashOnSolutionEmptyComponent> entity, ref SolutionContainerChangedEvent args)
        {
            CheckSolutions(entity);
        }

        public void CheckSolutions(Entity<TrashOnSolutionEmptyComponent> entity)
        {
            if (!HasComp<SolutionContainerManagerComponent>(entity))
                return;

            if (_solutionContainerSystem.TryGetSolution(entity.Owner, entity.Comp.Solution, out _, out var solution))
                UpdateTags(entity, solution);
        }

        public void UpdateTags(Entity<TrashOnSolutionEmptyComponent> entity, Solution solution)
        {
            if (solution.Volume <= 0)
            {
                _tagSystem.AddTag(entity.Owner, TrashTag);
                return;
            }

            _tagSystem.RemoveTag(entity.Owner, TrashTag);
        }
    }
}
