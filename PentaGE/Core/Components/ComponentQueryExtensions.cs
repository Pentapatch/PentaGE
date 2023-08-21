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
        public static IEnumerable<T> GetEntitiesOf<T>(this IEnumerable<Component> source) =>
            source.Where(c => c.Entity is T).Cast<T>();

        /// <summary>
        /// Gets a component of a specific type attached to the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <returns>The component of the specified type, if found; otherwise, <see langword="null"/>.</returns>
        public static T? Get<T>(this IEnumerable<Component> source) where T : Component =>
            source.FirstOrDefault(c => c is T) as T;

        /// <summary>
        /// Gets a component of a specific type attached to the entity by its unique identifier.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <param name="id">The unique identifier of the component to retrieve.</param>
        /// <returns>The component of the specified type with the given ID, if found; otherwise, <see langword="null"/>.</returns>
        public static T? Get<T>(this IEnumerable<Component> source, Guid id) where T : Component =>
            source.FirstOrDefault(c => c is T && c.ID == id) as T;

        /// <summary>
        /// Gets a component attached to the entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the component to retrieve.</param>
        /// <returns>The component with the given ID, if found; otherwise, <see langword="null"/>.</returns>
        public static Component? Get(this IEnumerable<Component> source, Guid id) =>
            source.FirstOrDefault(c => c.ID == id);

        /// <summary>
        /// Gets all components of a specific type attached to the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <returns>A list of components of the specified type.</returns>
        public static IEnumerable<T> GetAll<T>(this IEnumerable<Component> source) where T : Component =>
            source.Where(c => c is T).Cast<T>();

        /// <summary>
        /// Tests if the entity has a component of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of component to test.</typeparam>
        /// <returns></returns>
        public static bool Has<T>(this IEnumerable<Component> source) where T : Component =>
            source.Get<T>() is not null;

        /// <summary>
        /// Tests if the entity has a component of a specific type.
        /// </summary>
        /// <param name="type">The type of component to test.</param>
        /// <returns></returns>
        public static bool Has(this IEnumerable<Component> source, Type type) =>
            source.Any(c => c.GetType() == type);
    }
}