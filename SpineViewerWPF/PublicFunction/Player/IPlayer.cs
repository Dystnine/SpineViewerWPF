using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


public interface IPlayer
{
    void Initialize();

    void LoadContent(ContentManager contentManager);


    void Update(GameTime gameTime);

    void Draw();

    void ChangeSet();

    void SizeChange();

    void Dispose();
}

