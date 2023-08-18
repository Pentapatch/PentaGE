using PentaGE.Core.Entities;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Provides extension methods for querying components.
    /// </summary>
    public static class ComponentQueryExtensions
    {
        /// <summary>
        /// Filters the source collection to return components of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of components to retrieve.</typeparam>
        /// <param name="source">The collection of components to filter.</param>
        /// <returns>An enumerable collection of components of the specified type.</returns>
        public static IEnumerable<T> Of<T>(this IEnumerable<Component> source) where T : Component =>
            source.OfType<T>();

        /// <summary>
        /// Gets the entities associated with the components in the source collection.
        /// </summary>
        /// <param name="source">The collection of components to get entities from.</param>
        /// <returns>An enumerable collection of entities associated with the components.</returns>
        public static IEnumerable<Entity> GetEntities(this IEnumerable<Component> source) =>
            source.Select(c => c.Entity!);

        /// <summary>
        /// Filters the source collection to return entities of the specified type associated with the source collection of components.
        /// </summary>
        /// <typeparam name="T">The type of entities to consider for filtering.</typeparam>
        /// <param name="source">The collection of components to filter.</param>
        /// <returns>An enumerable collection of filtered entities associated with the source collection of components.</returns>
        public static IEnumerable<Entity> GetEntitiesOf<T>(this IEnumerable<Component> source) =>
            source.Where(c => c.Entity is T).Select(c => c.Entity!);
    }
}